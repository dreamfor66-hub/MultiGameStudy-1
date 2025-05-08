using System.Collections.Generic;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Core
{
    public static class AttackCalculator
    {
        private static readonly List<IAttacker> casters = new List<IAttacker>();
        private static readonly Queue<HitInfo> hitQueue = new Queue<HitInfo>();

        static AttackCalculator()
        {
            GameCommandHitAttack.Listen(OnHitAttack);
        }

        public static void OnHitAttack(GameCommandHitAttack cmd)
        {
            hitQueue.Enqueue(cmd.HitInfo);
        }

        public static void Add(IAttacker attacker)
        {
            casters.Add(attacker);
        }

        public static void Remove(IAttacker attacker)
        {
            casters.Remove(attacker);
        }

        public static List<HitResultInfo> CastAll(ref List<HitResultInfo> results)
        {
            results.Clear();

            foreach (var attacker in casters)
                attacker.Cast();

            // while 안에서 hitQueue 에 추가될수 있음
            while (hitQueue.Count > 0)
            {
                var hit = hitQueue.Dequeue();
                var buffInfo = CalculateBuff(hit);
                var damageInfo = CalculateDamage(hit, buffInfo);

                var killed = false;
                if (hit.Main.Victim is CharacterBehaviour character)
                {
                    if (hit.Detail.IsInstantDeath)
                    {
                        killed = character.HpModule.InstantDeath();
                    }
                    else
                    {
                        killed = character.HpModule.Attack(damageInfo.FinalDamage);
                    }
                }

                var totalInfo = new HitTotalInfo
                {
                    Main = hit.Main,
                    Detail = hit.Detail,
                    Damage = damageInfo,
                };

                BuffTriggerDispatcher.AttackHit(totalInfo);
                if (killed)
                    BuffTriggerDispatcher.Death(hit.Main.Attacker, hit.Main.Victim);

                var result = new HitResultInfo
                {
                    Main = hit.Main,
                    Damage = damageInfo,
                    Killed = killed,
                };
                results.Add(result);

                if (hit.Main.Victim is CharacterBehaviour character2)
                    character2.HurtToEntity(result);

                if (hit.Main.Attacker is CharacterBehaviour character3)
                    character3.HitToEntity(result);
                else if (hit.Main.AttackerRoot is CharacterBehaviour character4)
                    character4.HitToEntity(result);

            }

            return results;
        }

        public static HitBuffInfo CalculateBuff(HitInfo hitInfo)
        {
            var attackerBuffHolder = hitInfo.Main.AttackerRoot as CharacterBehaviour;
            var buffInfo = new HitBuffInfo();
            buffInfo.CriticalChance = hitInfo.CriticalChance;
            buffInfo.AdditionalCriticalDamagePercent = hitInfo.AdditionalCriticalDamagePercent;
            if (attackerBuffHolder != null)
            {
                buffInfo.AdditionalDamage += attackerBuffHolder.BuffValues.GetValue(BuffSimpleValueType.AttackDamage);
                buffInfo.AdditionalDamagePercent += attackerBuffHolder.BuffValues.GetValue(BuffSimpleValueType.AttackDamagePercent);
                attackerBuffHolder.BuffAccepter.OverrideHit(hitInfo, ref buffInfo);
            }
            var victimBuffHolder = hitInfo.Main.Victim as CharacterBehaviour;
            if (victimBuffHolder != null)
            {
                buffInfo.VictimAdditionalDamagePercent += victimBuffHolder.BuffValues.GetValue(BuffSimpleValueType.HurtDamagePercent);
                victimBuffHolder.BuffAccepter.OverrideHurt(hitInfo, ref buffInfo);
            }

            return buffInfo;
        }

        public static HitDamageInfo CalculateDamage(HitInfo hit, HitBuffInfo buffInfo)
        {
            var critical = (hit.Detail.DamageType == HitDamageType.Elemental ? false : Random.Range(0, 100) < buffInfo.CriticalChance);

            // AdditionalDamagePercent
            var normalDamage = (hit.Detail.DamageType == HitDamageType.Normal ? hit.BasicDamage : 0) * (1 + (float)buffInfo.AdditionalDamagePercent / 100) + buffInfo.AdditionalDamage;
            var trueDamage = (hit.Detail.DamageType == HitDamageType.True ? hit.BasicDamage : 0) + buffInfo.AdditionalTrueDamage * 1.0f;
            var elementalDamage = (hit.Detail.DamageType == HitDamageType.Elemental ? hit.BasicDamage : 0) * 1.0f;

            //VictimAdditionalDamagePercent
            normalDamage *= (1 + (float)buffInfo.VictimAdditionalDamagePercent / 100);
            elementalDamage *= (1 + (float)buffInfo.VictimAdditionalDamagePercent / 100);

            //Critical multiplier
            if (critical)
            {
                normalDamage *= (CommonVariables.BasicCriticalDamagePercent + buffInfo.AdditionalCriticalDamagePercent) / 100f;
                trueDamage *= (CommonVariables.BasicCriticalDamagePercent + buffInfo.AdditionalCriticalDamagePercent) / 100f;
            }

            var totalDamage = normalDamage + trueDamage + elementalDamage;
            var finalDamage = Mathf.Clamp((int)totalDamage, CommonVariables.DamageMin, CommonVariables.DamageMax);

            if (hit.BasicDamage == 0)
                finalDamage = 0;

            var dpDamage = 1;
            if (critical)
                dpDamage *= 2;
            if (hit.Detail.IsDirect)
                dpDamage *= 2;

            return new HitDamageInfo
            {
                IsCritical = critical,
                FinalDamage = finalDamage,
                DpDamage = dpDamage
            };
        }
    }
}