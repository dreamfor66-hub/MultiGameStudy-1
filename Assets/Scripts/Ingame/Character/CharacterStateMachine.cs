using System;
using System.Linq;
using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Event;
using Rogue.Ingame.Network;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class CharacterStateMachine
    {
        public CharacterStateInfo Info
        {
            get
            {
                var info = new CharacterStateInfo();
                info.Id = id;
                info.StateType = stateType;
                info.Frame = frame;
                info.Direction = direction;
                info.ActionData = actionData;
                info.KnockbackDistance = knockbackDistance;
                info.KnockbackStrength = knockbackStrength;
                info.KnockStopFrame = knockStopFrame;
                info.HitstopId = HitFxTable.Instance.GetId(hitstop);
                info.HitstopFrame = hitstopFrame;
                info.HitstopReductionIdx = hitstopReductionIdx;
                info.StunFrame = stunFrame;
                return info;
            }
        }

        private readonly CharacterBehaviour character;
        private readonly CharacterData characterData;
        private readonly IActionResource stamina;
        private readonly IActionResource stack;
        private readonly CoolTimeModule coolTime;
        private readonly Transform tm;
        private readonly Team team;

        private int id;
        private CharacterStateType stateType;
        private float frame;
        private VectorXZ direction;
        private ActionData actionData;
        private float knockbackDistance;
        private KnockbackStrength knockbackStrength;
        private int knockStopFrame;
        private HitFxData hitstop;
        private int hitstopReductionIdx;
        private int hitstopFrame;
        private int stunFrame;


        public CharacterStateMachine(CharacterBehaviour character, IActionResource stamina, IActionResource stack, CoolTimeModule coolTime)
        {
            this.character = character;
            this.characterData = character.characterData;
            this.tm = character.transform;
            this.team = character.team;
            this.stamina = stamina;
            this.stack = stack;
            this.coolTime = coolTime;
            id = 0;
            stateType = CharacterStateType.Idle;
            frame = 0f;
            direction = new VectorXZ(0f, 1f);
            knockbackDistance = 0f;
            knockbackStrength = KnockbackStrength.Low;
            knockStopFrame = 0;
            actionData = null;
            hitstop = null;
            hitstopFrame = 0;
            hitstopReductionIdx = 0;
            stunFrame = 0;

            if (characterData.AppearAction != null)
                ToAction(characterData.AppearAction);
        }

        public void Sync(CharacterStateInfo stateInfo)
        {
            id = stateInfo.Id;
            stateType = stateInfo.StateType;
            frame = stateInfo.Frame;
            direction = stateInfo.Direction;
            actionData = stateInfo.ActionData;
            knockbackDistance = stateInfo.KnockbackDistance;
            knockbackStrength = stateInfo.KnockbackStrength;
            knockStopFrame = stateInfo.KnockStopFrame;
            hitstop = HitFxTable.Instance.GetById(stateInfo.HitstopId);
            hitstopFrame = stateInfo.HitstopFrame;
            hitstopReductionIdx = stateInfo.HitstopReductionIdx;
            stunFrame = stateInfo.StunFrame;
        }

        public void UpdateFrame(float attackSpeed, float moveSpeed, float totalSpeed)
        {
            var deltaFrame = 1f;
            if (hitstop != null)
            {
                hitstopFrame++;
                var reduce = HitstopReduce.GetReduce(hitstopReductionIdx);
                var max = hitstop != null ? hitstop.GetHitstopMaxFrame() * reduce : 0;
                if (hitstopFrame > max)
                    hitstop = null;
                else if (hitstop != null)
                {
                    var speed = hitstop.GetHitstopSpeed(hitstopFrame / reduce);
                    deltaFrame *= speed;
                }
            }

            deltaFrame *= totalSpeed;
            if (stateType == CharacterStateType.Run || stateType == CharacterStateType.WalkAround)
                deltaFrame *= moveSpeed;
            else if (stateType == CharacterStateType.Action && actionData.IsAttack)
                deltaFrame *= attackSpeed;
            frame += deltaFrame;
            if (stunFrame > 0)
                stunFrame--;
        }

        public bool DoCommand(CharacterCommandDirection commandDirection)
        {
            var command = commandDirection.Command;
            var dir = commandDirection.Direction;
            if (DoCommand(command, dir))
            {
                if (dir != VectorXZ.Zero)
                    direction = dir.Normalized;
                if (stateType == CharacterStateType.Action && actionData.EnableAutoCorrection)
                {
                    direction = AutoCorrection.CorrectedDirection(tm.position, direction, team,
                        actionData.AutoCorrectionMaxDistance, actionData.AutoCorrectionDefaultDistance, actionData.AutoCorrectionMaxAngle, actionData.AutoCorrectionMinAngle);
                }
                return true;
            }
            else
                return false;
        }

        public bool DoCommand(CharacterCommandType command, VectorXZ direction)
        {
            var hasDirection = direction.Magnitude > 0.05f;
            switch (stateType)
            {
                case CharacterStateType.Idle:
                case CharacterStateType.Run:
                case CharacterStateType.WalkAround:
                    return DoCommand(command, "", true, false, hasDirection);
                case CharacterStateType.Action:
                    {
                        var keywords = actionData.CustomStates.Where(x => x.StartFrame <= frame && frame <= x.EndFrame)
                            .Select(x => x.Key);
                        foreach (var key in keywords)
                        {
                            if (DoCommand(command, key, false, false, hasDirection))
                                return true;
                        }

                        if (actionData.ExitableFrame <= frame)
                        {
                            if (DoCommand(command, "", true, false, hasDirection))
                                return true;
                        }
                        return false;
                    }
                case CharacterStateType.Hurt:
                    {
                        var knockbackFrame = KnockbackCalculator.TotalTime(knockbackDistance) * CommonVariables.GameFrame;
                        if (KnockbackCalculator.IsToGetup(knockbackStrength) && frame > knockbackFrame - CommonVariables.KnockbackEvadableFrame)
                            if (DoCommand(command, "__Getup__", false, true, hasDirection))
                                return true;
                        return false;
                    }
                case CharacterStateType.Dead:
                case CharacterStateType.Stun:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



        public void UpdateCommand(VectorXZ dir, bool isWalk)
        {
            if (dir != VectorXZ.Zero && (stateType == CharacterStateType.Idle || stateType == CharacterStateType.Run || stateType == CharacterStateType.WalkAround))
            {
                direction = dir.Normalized;
            }
            switch (stateType)
            {
                case CharacterStateType.Idle:
                    if (dir != VectorXZ.Zero)
                        ToMoveState(isWalk);
                    break;
                case CharacterStateType.Run:
                    if (dir == VectorXZ.Zero)
                        ToState(CharacterStateType.Idle);
                    else if (isWalk)
                        ToState(CharacterStateType.WalkAround);
                    break;
                case CharacterStateType.WalkAround:
                    if (dir == VectorXZ.Zero)
                        ToState(CharacterStateType.Idle);
                    else if (!isWalk)
                        ToState(CharacterStateType.Run);
                    break;
                case CharacterStateType.Action:
                    if (frame > actionData.ExitableFrame && dir != VectorXZ.Zero)
                        ToMoveState(isWalk);
                    else if (frame > actionData.TotalFrame)
                        ToState(CharacterStateType.Idle);

                    if (IsFixedDirRotateState(out var speed))
                    {
                        if (dir.Normalized != VectorXZ.Zero)
                        {
                            UpdateRotation(dir, speed * 1f);
                        }
                    }
                    break;
                case CharacterStateType.Hurt:
                    var knockbackFrame = KnockbackCalculator.TotalTime(knockbackDistance) * CommonVariables.GameFrame;
                    var totalFrame = knockbackFrame + GetKnockbackAdditionalFrame();
                    if (frame > totalFrame)
                    {
                        if (stunFrame > 0)
                            ToState(CharacterStateType.Stun);
                        else
                            ToState(CharacterStateType.Idle);
                    }
                    break;
                case CharacterStateType.Dead:
                    break;
                case CharacterStateType.Stun:
                    if (stunFrame <= 0)
                        ToState(CharacterStateType.Idle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsFixedDirRotateState(out float speed)
        {
            speed = 0f;
            if (stateType != CharacterStateType.Action)
                return false;
            foreach (var rot in actionData.MoveData)
            {
                if (rot.StartFrame <= frame && frame <= rot.EndFrame && rot.FixedRotSpeed > 0f)
                {
                    speed = rot.FixedRotSpeed;
                    return true;
                }
            }
            return false;
        }
        private void UpdateRotation(VectorXZ dir, float rotation)
        {
            var angle = Vector3.SignedAngle(direction, dir, Vector3.up);
            var directions = Quaternion.AngleAxis(rotation * Mathf.Sign(angle), Vector3.up) * direction;
            if (Mathf.Abs(angle) <= rotation)
                direction = dir;
            else
                direction = new VectorXZ(directions);
        }

        public bool DoCommand(CharacterCommandType command, string stateKey, bool fromIdle, bool fromGetUp, bool hasDirection)
        {
            var next = FindNextAction(command, stateKey, fromIdle, fromGetUp, hasDirection);
            if (next != null)
            {
                UseResource(next);
                ToAction(next);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DoCommand(ActionData action, VectorXZ vec)
        {
            if (action == null)
                return;
            if (stateType == CharacterStateType.Dead || stateType == CharacterStateType.Stun)
                return;
            if (stateType == CharacterStateType.Hurt && !action.CanBeInHurt)
                return;

            ToAction(action);
            if (vec != VectorXZ.Zero)
                direction = vec.Normalized;
        }

        private bool CheckResource(ActionData action)
        {
            return stamina.CanUse(action.StaminaUse) && stack.CanUse(action.StackUse);
        }

        private void UseResource(ActionData action)
        {
            stamina.Use(action.StaminaUse);
            stack.Use(action.StackUse);
        }

        private void ToAction(ActionData data)
        {
            if (data == null)
                throw new Exception("To null action");
            if (!characterData.PossibleActions.Contains(data))
                Debug.LogError($"Possible Actions 에 포함되어 있지 않은 Action 을 재생합니다. 확인해주세요. : {characterData.name} - {data.name}");
            NextId();
            stateType = CharacterStateType.Action;
            frame = 0f;
            actionData = data;

            if (actionData.IsAttack)
                EventDispatcher.Send(new EventAttackDo(character, tm, actionData.ActionRange, 0));
        }

        private void ToMoveState(bool isWalk)
        {
            if (isWalk)
                ToState(CharacterStateType.WalkAround);
            else
                ToState(CharacterStateType.Run);
        }

        private void ToState(CharacterStateType state)
        {
            NextId();
            stateType = state;
            frame = 0f;
            actionData = null;
        }

        public void Hurt(KnockbackInfo knockbackInfo, bool isDie)
        {
            if (stateType == CharacterStateType.Dead)
                return;

            if (!isDie && knockbackInfo.Strength == KnockbackStrength.JustDamage)
                return;

            NextId();
            stateType = isDie ? CharacterStateType.Dead : CharacterStateType.Hurt;
            frame = 0f;
            actionData = null;
            direction = new VectorXZ(-knockbackInfo.Direction).Normalized;
            knockbackDistance = knockbackInfo.Distance;
            knockbackStrength = knockbackInfo.Strength;
            knockStopFrame = knockbackInfo.KnockStopFrame;
        }

        public void Hitstop(HitstopInfo info)
        {
            if (!info.IsValid())
                return;
            hitstop = HitFxTable.Instance.GetByIdOrDefault(info.HitFxId);
            hitstopReductionIdx = info.ReductionIdx;
            hitstopFrame = -info.AdditionalFrame;
        }

        public void SetDirection(VectorXZ dir)
        {
            this.direction = dir.Normalized;
        }

        public void Stun(int newFrame)
        {
            stunFrame = Mathf.Max(stunFrame, newFrame);

            if (stateType != CharacterStateType.Hurt && stateType != CharacterStateType.Dead)
                if (stunFrame > 0)
                    ToState(CharacterStateType.Stun);
        }

        public void ForceAction(ActionData action)
        {
            if (stateType == CharacterStateType.Dead)
                return;
            ToAction(action);
        }

        public void Revive()
        {
            if (stateType == CharacterStateType.Dead)
                ToState(CharacterStateType.Idle);
        }

        private void NextId()
        {
            id = (id + 1) % 256;
        }

        private ActionData FindNextAction(CharacterCommandType command, string stateKey, bool fromIdle, bool fromGetup, bool hasDirection)
        {
            foreach (var action in characterData.CommandToActions)
            {
                if (action.Command != command)
                    continue;

                if (!CheckResource(action.Action))
                    continue;

                if (!CheckDirection(action.DirectionMask, hasDirection))
                    continue;

                // CharacterStateMachine.cs 내부 FindNextAction 에 아래 로그 추가
                Debug.Log($"Checking command: {command}, matched: {action.Command}, stateKey: {action.StateKey}");


                if (coolTime.RemainFrame(action.Action, ServerFrameHolder.Frame) > 0) 
                    continue;

                if (fromIdle && action.FromIdle)
                    return action.Action;

                if (fromGetup && action.FromGetup)
                    return action.Action;

                if (stateKey == action.StateKey)
                    return action.Action;

            }
            return null;
        }

        private bool CheckDirection(DirectionMask mask, bool hasDirection)
        {
            if (hasDirection)
                return (mask & DirectionMask.Direction) != 0;
            else
                return (mask & DirectionMask.Neutral) != 0;
        }

        private int GetKnockbackAdditionalFrame()
        {
            switch (knockbackStrength)
            {
                case KnockbackStrength.Low:
                    return knockStopFrame;
                case KnockbackStrength.Mid:
                    return characterData.KnockbackMidGetUpFrame;
                case KnockbackStrength.High:
                    return characterData.KnockbackHighGetUpFrame;
                case KnockbackStrength.JustDamage:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}