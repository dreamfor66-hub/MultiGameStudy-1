using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Random;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Bot
{
    public class BotPhaseModel
    {
        private readonly BotPhaseData phaseData;
        private readonly CharacterBehaviour character;
        private readonly HpModule hpModule;
        private readonly Transform me;
        private int phaseIdx;
        private int mainLoopIdx = 0;
        private int counterCooldown;
        private int evadeCooldown;
        private int counterRage;
        private readonly List<CharacterBehaviour> hurts = new List<CharacterBehaviour>();
        private readonly BotPatternScoreModel scoreModel;

        public int PhaseIdx => phaseIdx;
        public BotSinglePhaseData CurPhase => phaseData.Phases[phaseIdx];
        public BotPhasePatternPoolData SelectPhasePattern(float distance) => GetPattern(PatternPoolTag.Phase, distance);


        public BotPhaseModel(BotPhaseData phaseData, CharacterBehaviour character, BotPatternScoreModel scoreModel)
        {
            this.phaseData = phaseData;
            this.character = character;
            this.hpModule = character.HpModule;
            this.scoreModel = scoreModel;
            me = character.transform;
            phaseIdx = 0;
        }

        public bool NeedToGoNextPhase()
        {
            var hasNextPhase = phaseData.Phases.Count > phaseIdx + 1;
            var hpCondition = hpModule.HpInfo.Ratio <= CurPhase.NextCondition.HpLessThan;
            return hpCondition && hasNextPhase;
        }

        public void NextPhase()
        {
            phaseIdx++;
            mainLoopIdx = 0;
            counterCooldown = 0;
            counterRage = 0;
            evadeCooldown = 0;
        }

        public void UpdateFrame()
        {
            hurts.Clear();
            counterCooldown--;
            evadeCooldown--;
        }

        public void Hurt(CharacterBehaviour attacker)
        {
            hurts.Add(attacker);
        }

        public bool CheckAndGetCounter(out BotPhasePatternPoolData counterPattern, out CharacterBehaviour target)
        {
            target = null;
            counterPattern = null;

            foreach (var attacker in hurts)
            {
                if (attacker == null)
                    continue;
                if (++counterRage + Random.Range(0, 2) < CurPhase.CounterByRage)
                {
                    if (counterCooldown > 0)
                        continue;

                    var rand = Random.Range(0f, 1f);
                    if (rand > CurPhase.CounterChance)
                        continue;
                }

                target = attacker;
                var dist = (target.transform.position - me.position).magnitude;
                counterPattern = GetPattern(PatternPoolTag.Counter, dist);
                if (counterPattern == null)
                    return false;

                counterCooldown = CurPhase.CounterCooldownFrame;
                evadeCooldown = CurPhase.EvadeCooldownFrame;
                counterRage = 0;

                //메인루프 Random 도중에 친 카운터는 Random 메인패턴 재생한 걸로 간주한다
                if (CurPhase.MainLoop[mainLoopIdx] == PatternPoolTag.Random)
                    mainLoopIdx = (mainLoopIdx + 1) % CurPhase.MainLoop.Count;

                return true;
            }

            return false;
        }

        public bool GetEvade(out BotPhasePatternPoolData evadePattern)
        {
            evadePattern = null;
            if (evadeCooldown > 0)
                return false;
            var rand = Random.Range(0f, 1f);
            if (rand > CurPhase.EvadeChance)
                return false;
            evadePattern = GetPattern(PatternPoolTag.Evade, 0);
            if (evadePattern == null)
                return false;

            evadeCooldown = CurPhase.EvadeCooldownFrame;
            return true;
        }

        public BotPhasePatternPoolData SelectPattern(float distance)
        {
            counterRage = 0;

            if (CurPhase.MainLoop.Count > 0)
            {
                var pattern = GetPattern(CurPhase.MainLoop[mainLoopIdx], distance);
                if (pattern != null)
                {
                    mainLoopIdx = (mainLoopIdx + 1) % CurPhase.MainLoop.Count;
                    return pattern;
                }
            }

            return GetPattern(PatternPoolTag.Random, distance);
        }

        public bool IsMainLoopAvailable()
        {
            if (CurPhase.MainLoop[mainLoopIdx] == PatternPoolTag.Random)
                return false;
            return CurPhase.PatternPool.Where(x => x.Tag == CurPhase.MainLoop[mainLoopIdx]).Any(x => IsConditionSatisfied(x.Pattern));
        }

        private bool IsConditionSatisfied(BotPatternData pattern)
        {
            return pattern.Conditions.All(IsConditionSatisfied);
        }

        private bool IsConditionSatisfied(BotPatternConditionData conditionData)
        {
            switch (conditionData.Type)
            {
                case BotPatternConditionType.None:
                    return true;
                case BotPatternConditionType.HasBuff:
                    return character.BuffAccepter.HasBuff(conditionData.BuffTag);
                case BotPatternConditionType.NotHasBuff:
                    return !character.BuffAccepter.HasBuff(conditionData.BuffTag);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private BotPhasePatternPoolData GetPattern(PatternPoolTag tag, float distance)
        {
            var pool = CurPhase.PatternPool.Where(x => x.Tag == tag).Where(x => IsConditionSatisfied(x.Pattern)).ToArray();
            var idx = RandomSelector.Select(RandomByUnity.Instance, pool.Select(x => x.Pattern.CalibratedChance(distance, x.Chance) + scoreModel.GetChanceDiff(x)));
            return idx >= 0 ? pool[idx] : null;
        }
    }
}