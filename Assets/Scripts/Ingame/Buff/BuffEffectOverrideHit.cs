using System;
using System.Linq;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff
{
    public class BuffEffectOverrideHit
    {
        private readonly BuffEffectOverrideHitData data;
        private readonly BuffInstance buff;

        public BuffEffectOverrideHit(BuffEffectOverrideHitData data, BuffInstance buff)
        {
            this.data = data;
            this.buff = buff;
        }

        public void OverrideHit(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            if (data.Situation != HitSituation.Hit)
                return;
            if (!CheckFilter(hitInfo.Main.Attacker, hitInfo.Main.Victim, hitInfo))
                return;
            ApplyOverride(hitInfo, ref buffInfo);
        }

        public void OverrideHurt(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            if (data.Situation != HitSituation.Hurt)
                return;
            if (!CheckFilter(hitInfo.Main.Attacker, hitInfo.Main.Victim, hitInfo))
                return;
            ApplyOverride(hitInfo, ref buffInfo);
        }


        private bool CheckFilter(IEntity attacker, IEntity victim, HitInfo hitInfo)
        {
            return data.Filters.All(x => CheckFilter(x, attacker, victim, hitInfo));
        }

        private bool CheckFilter(HitFilterData filter, IEntity attacker, IEntity victim, HitInfo hitInfo)
        {
            switch (filter.Type)
            {
                case HitFilterType.TargetMoveSlow:
                    {
                        if (victim is CharacterBehaviour character)
                            return character.BuffValues.MoveSpeedPercent < 0;
                        else
                            return false;
                    }
                case HitFilterType.TargetIsBoss:
                    {
                        if (victim is CharacterBehaviour character)
                            return character.CharacterData.IsBoss;
                        else
                            return false;
                    }
                case HitFilterType.TargetAbnormalStatus:
                    {
                        if (victim is CharacterBehaviour character)
                            return character.BuffAccepter.IsAbnormalStatus();
                        else
                            return false;
                    }
                default:
                    throw new NotImplementedException($"not implemented hit filter : {filter.Type}");
            }
        }

        private void ApplyOverride(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            switch (data.Override.Type)
            {
                case HitOverrideType.IncreaseDamagePercent:
                    {
                        buffInfo.AdditionalDamagePercent += data.Override.Value;
                        break;
                    }
                case HitOverrideType.IncreaseTrueDamageValuePerStack:
                    {
                        var addDamage = buff.StackCount * data.Override.Value;
                        buffInfo.AdditionalTrueDamage += addDamage;
                        break;
                    }
                case HitOverrideType.IncreaseDamagePercentByTargetHpRatioCurve:
                    {
                        if (hitInfo.Main.Victim is CharacterBehaviour character)
                        {
                            var hpModule = character.HpModule;
                            var hpRatio = hpModule.HpInfo.Ratio;
                            buffInfo.AdditionalDamage +=
                                (int)(hitInfo.BasicDamage * data.Override.Curve.Evaluate(hpRatio));
                        }
                        break;
                    }
                case HitOverrideType.CriticalChance:
                    {
                        buffInfo.CriticalChance += data.Override.Value;
                        break;
                    }
                default:
                    throw new NotImplementedException($"not implemented hit override : {data.Override.Type}");
            }
        }


    }
}