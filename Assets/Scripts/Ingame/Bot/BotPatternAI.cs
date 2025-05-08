using System;
using System.Threading.Tasks;
using FMLib.Structs;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.EntityMessage;
using Rogue.Ingame.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Bot
{
    public enum BotAiStateType
    {
        Wait,
        RandomMove,

        Pattern,
    }

    public class BotPatternAi
    {
        private readonly CharacterBehaviour character;

        public BotTargetModel TargetModel => targetModel;
        public BotPatternScoreModel ScoreModel => scoreModel;
        //state
        private BotAiStateType stateType;
        private int stateFrame;
        private VectorXZ stateDirection;
        private readonly BotPatternPlayer patternPlayer;
        private BotPhasePatternPoolData curPattern;

        private bool isFreezed;
        private bool needEvade;
        private CharacterBehaviour evadeTarget;

        public IEntity Target => targetModel.Target;
        public int PhaseIdx => phaseModel.PhaseIdx;

        private readonly BotTargetModel targetModel;
        private readonly BotPhaseModel phaseModel;
        private readonly BotPatternScoreModel scoreModel;

        public BotPatternAi(CharacterBehaviour character, BotTargetInfo targetInfo, BotPhaseData phaseData, AggroVariables aggroVariables, BotPatternScoreVariables scoreVariables, bool isFreezed)
        {
            this.character = character;
            this.isFreezed = isFreezed;
            targetModel = new BotTargetModel(targetInfo, character.transform, aggroVariables);
            scoreModel = new BotPatternScoreModel(scoreVariables);
            phaseModel = new BotPhaseModel(phaseData, character, scoreModel);
            patternPlayer = new BotPatternPlayer(character, targetModel);
            character.MessageDispatcher.Listen<EntityMessageHurt>(Hurt);
            character.MessageDispatcher.Listen<EntityMessageHit>(Hit);
            character.MessageDispatcher.Listen<EntityMessageAggro>(Aggro);
            EventDispatcher.Listen<EventAttackDo>(Evade);
            ToWait(120);
        }

        private async void Hurt(EntityMessageHurt hurt)
        {
            if (!hurt.HitResultInfo.Main.IsDirect)
                return;
            await Task.Delay(30); // FIXME : 쉬운 구현을 위해 대충 뭉갰음 
            if (stateType == BotAiStateType.Wait)
                stateFrame = Math.Min(stateFrame, Math.Max((int)(stateFrame * (1 - phaseModel.CurPhase.WaitDecayByHurt)), 30));
            if (hurt.HitResultInfo.Main.Attacker is CharacterBehaviour attacker)
            {
                phaseModel.Hurt(attacker);
                targetModel.Hurt(attacker, stateType);
                scoreModel.Hurt(hurt.HitResultInfo.Damage.FinalDamage, stateType);
            }
            else if (hurt.HitResultInfo.Main.AttackerRoot is CharacterBehaviour attacker2)
            {
                phaseModel.Hurt(attacker2);
                targetModel.Hurt(attacker2, stateType);
                scoreModel.Hurt(hurt.HitResultInfo.Damage.FinalDamage, stateType);
            }
        }

        private void Hit(EntityMessageHit hit)
        {
            if (hit.HitResultInfo.Main.Victim is CharacterBehaviour target)
            {
                targetModel.Hit(target);
                scoreModel.Hit(hit.HitResultInfo.Damage.FinalDamage);
            }
        }

        private void Aggro(EntityMessageAggro aggro)
        {
            if (aggro.Target is CharacterBehaviour target)
                targetModel.BuffGainAggro(target, aggro.Value);
        }

        private void Evade(EventAttackDo attack)
        {
            //FIXME: 대충 구현함
            var dir = character.transform.position - attack.Pivot.position;
            var minRange = Vector3.Dot(attack.Pivot.transform.forward * attack.Range.x, dir.normalized);
            var maxRange = Vector3.Dot(attack.Pivot.transform.forward * attack.Range.y, dir.normalized);
            if (Mathf.Clamp(dir.magnitude, minRange, maxRange) == dir.magnitude && stateType == BotAiStateType.Wait)
            {
                needEvade = true;
                //TODO: 좀 더 깔끔하게
                evadeTarget = attack.Character;
            }
        }

        public void ToWait(int frame)
        {
            stateType = BotAiStateType.Wait;
            if (!isFreezed)
                targetModel.SetTarget(targetModel.GetAggroTarget(0));
            stateFrame = frame;
        }

        public void ToPattern(BotPhasePatternPoolData patternPoolData)
        {
            stateType = BotAiStateType.Pattern;
            if (patternPoolData != null)
                patternPlayer.Reset(patternPoolData.Pattern);
            curPattern = patternPoolData;

            scoreModel.Change(curPattern);
        }

        private void ToRandomMove()
        {
            stateType = BotAiStateType.RandomMove;
            stateDirection = RandomDirection();
            stateFrame = Random.Range(60, 120);
        }

        private VectorXZ RandomDirection()
        {
            var angle = Random.Range(0f, 2 * Mathf.PI);
            return new VectorXZ(Mathf.Sin(angle), Mathf.Cos(angle));
        }

        public void UpdateState()
        {
            targetModel.UpdateFrame();
            switch (stateType)
            {
                case BotAiStateType.Wait:
                    UpdateWait();
                    break;
                case BotAiStateType.RandomMove:
                    UpdateRandomMove();
                    break;
                case BotAiStateType.Pattern:
                    UpdatePattern();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            phaseModel.UpdateFrame();
            scoreModel.UpdateFrame();
        }

        public BotStateInfo GetInfo()
        {
            switch (stateType)
            {
                case BotAiStateType.Wait:
                    if (targetModel.Target != null)
                        return BotStateInfo.CreateWalkAround(targetModel.Target);
                    else
                        return BotStateInfo.CreateWait();
                case BotAiStateType.RandomMove:
                    return BotStateInfo.CreateMoveDirection(stateDirection);
                case BotAiStateType.Pattern:
                    return patternPlayer.GetInfo();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateWait()
        {
            stateFrame--;
            if (phaseModel.NeedToGoNextPhase())
            {
                var dist = UpdateTargetAndGetDistance();
                phaseModel.NextPhase();
                scoreModel.UpdateScore(phaseModel.CurPhase);
                var phasePattern = phaseModel.SelectPhasePattern(dist);
                if (phasePattern != null)
                    ToPattern(phasePattern);
            }
            else if (phaseModel.IsMainLoopAvailable())
            {
                var dist = UpdateTargetAndGetDistance();
                ToPattern(phaseModel.SelectPattern(dist));
            }
            else if (needEvade && phaseModel.GetEvade(out var evadePattern))
            {
                needEvade = false;
                targetModel.Taunt(evadeTarget);
                ToPattern(evadePattern);
            }
            else if (phaseModel.CheckAndGetCounter(out var counterPattern, out var target))
            {
                targetModel.Taunt(target);
                ToPattern(counterPattern);
            }
            else if (stateFrame <= 0)
            {
                var dist = UpdateTargetAndGetDistance();
                ToPattern(phaseModel.SelectPattern(dist));
            }

            needEvade = false;
        }

        private float UpdateTargetAndGetDistance()
        {
            targetModel.UpdateAggroList();
            var firstTarget = targetModel.GetAggroTarget(0);
            if (firstTarget != null)
                return Vector3.Distance(firstTarget.transform.position, character.transform.position);
            else
                return 0f;
        }

        private void UpdateRandomMove()
        {
            stateFrame--;
            if (stateFrame <= 0)
            {
                ToWait(Random.Range(60, 120));
            }
        }

        private void UpdatePattern()
        {
            patternPlayer.UpdateFrame();
            if (patternPlayer.IsEnd)
                ToWait(Random.Range(curPattern.WalkFrameMin, curPattern.WalkFrameMax));
        }

        public void OnDestroy()
        {
            EventDispatcher.Remove<EventAttackDo>(Evade);
        }
    }
}