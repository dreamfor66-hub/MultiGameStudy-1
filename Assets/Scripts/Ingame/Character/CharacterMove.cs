using System;
using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Rogue.Ingame.Character
{
    public class CharacterMove
    {
        private readonly CharacterData characterData;
        private readonly CharacterController characterController;
        private readonly CharacterBehaviour character;

        public CharacterMove(CharacterData characterData, CharacterController characterController, CharacterBehaviour characterBehaviour)
        {
            this.characterData = characterData;
            this.characterController = characterController;
            this.character = characterBehaviour;
        }



        public void UpdateMove(CharacterStateUpdateInfo update, VectorXZ controlDir)
        {
            var direction = update.Cur.Direction;
            if (update.Cur.StateType == CharacterStateType.WalkAround && character.Target != null)
                direction = new VectorXZ(character.Target.GameObject.transform.position - character.transform.position).Normalized;
            if (update.Cur.StateType == CharacterStateType.WalkAround)
            {
                UpdateRotation(direction, characterData.WalkRotateSpeed, update.DeltaTime);
            }
            else if (IsLookingTargetState(update, out var targetDir))
            {
                UpdateRotation(targetDir, characterData.RotateSpeed, update.DeltaTime);
            }
            else
            {
                UpdateRotation(direction, characterData.RotateSpeed, update.DeltaTime);
            }

            switch (update.Cur.StateType)
            {
                case CharacterStateType.Idle:
                    break;
                case CharacterStateType.Run:
                    Move((Vector3)update.Cur.Direction * characterData.Speed * update.DeltaTime);
                    break;
                case CharacterStateType.WalkAround:
                    {
                        var deltaFrame = update.DeltaFrame;
                        if (update.Prev.Frame < characterData.WalkStartFrame)
                        {
                            deltaFrame = Mathf.Max(update.Cur.Frame - characterData.WalkStartFrame, 0);
                        }
                        var deltaTime = deltaFrame / CommonVariables.GameFrame;
                        Move((Vector3)update.Cur.Direction * characterData.WalkSpeed * deltaTime);
                        break;
                    }
                case CharacterStateType.Action:
                    {
                        var z = GetMove(update.Cur.ActionData.MoveCurve, update.Cur.Frame, update.DeltaFrame);
                        var x = GetMove(update.Cur.ActionData.MoveCurveX, update.Cur.Frame, update.DeltaFrame);
                        var forward = (Vector3)update.Cur.Direction;
                        var right = new Vector3(forward.z, 0f, -forward.x);
                        var move = forward * z + right * x;

                        if (IsFixedDirMoveState(update.Cur, out var fixedDirMoveSpeed))
                        {
                            move = (Vector3)controlDir * (fixedDirMoveSpeed * update.DeltaFrame / CommonVariables.GameFrame);
                        }
                        else if (character.Target != null && IsFollowState(update.Cur, out var followSpeed))
                        {
                            var del = character.Target.GameObject.transform.position - character.transform.position;
                            var max = followSpeed * update.DeltaFrame / CommonVariables.GameFrame;
                            if (del.magnitude < max)
                                move = del;
                            else
                                move = del.normalized * max;
                        }
                        Move(move);
                        break;
                    }
                case CharacterStateType.Hurt:
                case CharacterStateType.Dead:
                    {
                        var curFrame = update.Cur.Frame;
                        var prevFrame = curFrame - update.DeltaFrame;
                        var dist = update.Cur.KnockbackDistance;
                        var dir = -update.Cur.Direction;
                        var prevDist = KnockbackCalculator.FrameToDist(dist, prevFrame);
                        var curDist = KnockbackCalculator.FrameToDist(dist, curFrame);
                        var move = (curDist - prevDist) * (Vector3)dir;
                        Move(move);
                        break;
                    }
                case CharacterStateType.Stun:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        bool IsLookingTargetState(CharacterStateUpdateInfo update, out VectorXZ targetDir)
        {
            if (character.Target == null)
            {
                targetDir = VectorXZ.Zero;
                return false;
            }

            var frame = update.Cur.Frame;
            if (update.Cur.ActionData != null)
            {
                foreach (var move in update.Cur.ActionData.MoveData)
                {
                    if (move.StartFrame <= frame && frame <= move.EndFrame)
                    {
                        if (move.LookingTarget)
                        {
                            targetDir = new VectorXZ(character.Target.GameObject.transform.position - character.transform.position);
                            return true;
                        }
                    }
                }
            }
            targetDir = VectorXZ.Zero;
            return false;
        }

        private float GetMove(AnimationCurve curve, float curFrame, float deltaFrame)
        {
            var curTime = curFrame / CommonVariables.GameFrame;
            var prevTime = (curFrame - deltaFrame) / CommonVariables.GameFrame;
            return curve.Evaluate(curTime) - curve.Evaluate(prevTime);
        }


        private void UpdateRotation(Vector3 dir, float rotateSpeed, float deltaTime)
        {
            var curDir = characterController.transform.forward;
            var angle = Vector3.SignedAngle(curDir, dir, Vector3.up);
            var canAngle = rotateSpeed * deltaTime;
            if (Mathf.Abs(angle) < canAngle)
                characterController.transform.forward = dir;
            else
            {
                var newDir = Quaternion.AngleAxis(canAngle * Mathf.Sign(angle), Vector3.up) * curDir;
                characterController.transform.forward = newDir;
            }
        }

        private bool IsFixedDirMoveState(CharacterStateInfo stateInfo, out float speed)
        {
            speed = 0f;
            if (stateInfo.StateType != CharacterStateType.Action)
                return false;
            var action = stateInfo.ActionData;
            var frame = stateInfo.Frame;
            foreach (var move in action.MoveData)
            {
                if (move.StartFrame <= frame && frame <= move.EndFrame && move.FixedDirMoveSpeed > 0f)
                {
                    speed = move.FixedDirMoveSpeed;
                    return true;
                }
            }

            return false;
        }

        private bool IsFollowState(CharacterStateInfo stateInfo, out float speed)
        {
            speed = 0f;
            if (stateInfo.StateType != CharacterStateType.Action)
                return false;
            var action = stateInfo.ActionData;
            var frame = stateInfo.Frame;
            foreach (var move in action.MoveData)
            {
                if (move.StartFrame <= frame && frame <= move.EndFrame && move.FollowTargetSpeed > 0f)
                {
                    speed = move.FollowTargetSpeed;
                    return true;
                }
            }

            return false;
        }

        private void Move(Vector3 delta)
        {
            var tmPos = characterController.transform.position;
            if (NavMesh.SamplePosition(tmPos + delta, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                delta = hit.position - tmPos;
            }

            characterController.Move(delta);
        }
    }
}