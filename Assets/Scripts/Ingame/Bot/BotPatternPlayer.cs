using System;
using System.Collections.Generic;
using System.Data;
using FMLib.Structs;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Stage;
using UnityEngine;

namespace Rogue.Ingame.Bot
{
    public enum PatternState
    {
        MoveToTarget,
        StartAction,
        Action,
        Wait,
    }

    public class BotPatternPlayer
    {
        private CharacterBehaviour character;
        private BotPatternData patternData;
        private readonly BotTargetModel targetModel;
        private int actionIdx;
        private PatternState state;
        private int stateFrame;
        private IEntity target;
        private BotPatternActionData CurAction =>
            actionIdx < patternData.Actions.Count ? patternData.Actions[actionIdx] : null;


        public BotPatternPlayer(CharacterBehaviour character, BotTargetModel targetModel)
        {
            this.character = character;
            this.targetModel = targetModel;
        }

        public bool IsEnd => actionIdx >= patternData.Actions.Count;

        public void Reset(BotPatternData data)
        {
            patternData = data;
            actionIdx = 0;
            state = PatternState.MoveToTarget;
            if (CurAction != null)
            {
                target = targetModel.GetTarget(data.Actions[0].TargetType);
                targetModel.SetTarget(target);
            }
        }


        public void UpdateFrame()
        {
            if (patternData == null)
                return;
            if (actionIdx >= patternData.Actions.Count)
                return;

            if (state == PatternState.MoveToTarget)
            {
                if (target == null)
                {
                    if (CurAction.TargetType == BotPatternTargetType.MapCenter)
                    {
                        targetModel.SetTarget(null);
                        var center = MapCenter.Center != null ? MapCenter.Center.position : Vector3.zero;
                        if (Vector3.Distance(character.transform.position, center) <= CurAction.MinRange)
                            state = PatternState.StartAction;
                    }
                    else
                    {
                        state = PatternState.StartAction;
                    }
                }
                else if (Vector3.Distance(character.transform.position, target.GameObject.transform.position) <= CurAction.MinRange)
                {
                    state = PatternState.StartAction;
                }
            }
            else if (state == PatternState.StartAction)
            {
                stateFrame = 10;
                state = PatternState.Action;
                targetModel.DoAction(target, CurAction.TargetReduceAggro);
            }
            else if (state == PatternState.Action)
            {
                stateFrame--;
                if (stateFrame <= 0 && IsNotActionState())
                {
                    state = PatternState.Wait;
                    stateFrame = CurAction.WaitFrame;
                }
            }
            else
            {
                stateFrame--;
                if (stateFrame <= 0)
                {
                    actionIdx++;
                    state = PatternState.MoveToTarget;
                    if (CurAction != null)
                    {
                        if (CurAction.ResetAggroRank)
                            targetModel.UpdateAggroList();
                        target = targetModel.GetTarget(CurAction.TargetType);
                        targetModel.SetTarget(target);
                    }
                }
            }
        }

        public BotStateInfo GetInfo()
        {
            switch (state)
            {
                case PatternState.MoveToTarget:
                    if (CurAction.TargetType == BotPatternTargetType.MapCenter)
                    {
                        var center = MapCenter.Center != null ? MapCenter.Center.position : Vector3.zero;
                        return BotStateInfo.CreateMoveDirection(new VectorXZ(center - character.transform.position));
                    }
                    else
                        return BotStateInfo.CreateMoveToTarget(target);

                case PatternState.StartAction:
                    return BotStateInfo.CreateAttack(target, CurAction.Action, true);
                case PatternState.Action:
                    return BotStateInfo.CreateAttack(target, CurAction.Action, false);
                case PatternState.Wait:
                    return BotStateInfo.CreateWait();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private bool IsNotActionState()
        {
            var stateInfo = character.Character.StateInfo;
            var nextFrame = stateInfo.Frame + 1;
            if (stateInfo.ActionData == null)
                return true;
            return nextFrame >= stateInfo.ActionData.ExitableFrame;
        }


    }
}