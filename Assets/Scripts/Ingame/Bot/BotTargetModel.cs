using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Extensions;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Bot
{
    public class BotTargetModel
    {
        public IEntity Target { get; private set; }
        public bool HasTarget => Target != null;
        public AggroTable AggroTable => aggroTable;

        private readonly Transform me;
        private readonly BotTargetInfo targetInfo;
        private readonly AggroTable aggroTable;
        private readonly List<CharacterBehaviour> aggroTargets = new List<CharacterBehaviour>();

        public BotTargetModel(BotTargetInfo targetInfo, Transform me, AggroVariables variables)
        {
            this.targetInfo = targetInfo;
            this.me = me;
            aggroTable = new AggroTable(variables);
        }

        public void SetTarget(IEntity target)
        {
            Target = target;
        }

        public void UpdateFrame()
        {
            aggroTable.UpdateFrame(Time.fixedDeltaTime);
            if (Target != null && !IsTargetable(Target))
                Target = null;
        }

        public void Hurt(CharacterBehaviour attacker, BotAiStateType state)
        {
            aggroTable.Hurt(attacker);
            if (state == BotAiStateType.Pattern)
                aggroTable.HurtWhileActive(attacker);
        }

        public void Hit(CharacterBehaviour target)
        {
            aggroTable.HitTarget(target);
        }

        public void DoAction(IEntity target, int reduceAggro)
        {
            SetTarget(target);
            if (target != null)
                aggroTable.DoPattern(target, reduceAggro);
        }

        public void BuffGainAggro(CharacterBehaviour target, float value)
        {
            aggroTable.BuffGainAggro(target, value);
        }

        public void Taunt(CharacterBehaviour target)
        {
            aggroTable.Taunt(target);
            UpdateAggroList();
        }

        public void UpdateAggroList()
        {
            aggroTargets.Clear();
            aggroTargets.AddRange(CharacterBehaviour.Characters.Where(IsTargetable));
            aggroTargets.Sort((a, b) => (int)((aggroTable.GetAggro(b) - aggroTable.GetAggro(a)) * 1000));
        }

        public IEntity GetTarget(BotPatternTargetType targetType)
        {
            switch (targetType)
            {
                case BotPatternTargetType.None:
                case BotPatternTargetType.MapCenter:
                    return null;
                case BotPatternTargetType.Aggro1:
                    return GetAggroTarget(0);
                case BotPatternTargetType.Aggro2:
                    return GetAggroTarget(1);
                case BotPatternTargetType.Aggro3:
                    return GetAggroTarget(2);
                case BotPatternTargetType.MinHpPercent:
                    return EntityTable.Entities.OfType<CharacterBehaviour>().Where(IsTargetable)
                        .MinBy(x => x.HpModule.HpInfo.Ratio);
                case BotPatternTargetType.Boss:
                    return EntityTable.Entities.OfType<CharacterBehaviour>()
                        .FirstOrDefault(x => x.characterData.IsBoss);
                case BotPatternTargetType.PivotObject:
                    return TargetPivot.Pivots.FirstOrDefault()?.Entity;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            }
        }

        public CharacterBehaviour GetAggroTarget(int idx)
        {
            aggroTargets.RemoveAll(x => x.IsDead);
            idx = Mathf.Min(idx, aggroTargets.Count - 1);
            return idx >= 0 ? aggroTargets[idx] : null;
        }

        private bool IsTargetable(IEntity target)
        {
            if (target is CharacterBehaviour character)
            {
                return target.Team == targetInfo.TargetableTeam && !character.Hide && !character.IsDead;
            }
            else
            {
                return true;
            }
        }

        private float Distance(Transform tm)
        {
            return Vector3.Distance(me.position, tm.position);
        }
    }
}