using System;
using FMLib.Structs;
using Rogue.Ingame.Character;
using Rogue.Ingame.Entity;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Bot
{
    public class BotController
    {
        private readonly NavMeshAgent agent;

        public BotController(NavMeshAgent agent)
        {
            this.agent = agent;
        }



        public CharacterControlActionInfo StateToControl(BotStateInfo stateInfo)
        {
            CharacterControlActionInfo info;
            switch (stateInfo.StateType)
            {
                case BotStateInfoType.Wait:
                    info.Direction = VectorXZ.Zero;
                    info.Action = null;
                    info.Walk = false;
                    return info;
                case BotStateInfoType.WalkAround:
                    info.Direction = WalkAround(stateInfo.Target);
                    info.Action = null;
                    info.Walk = true;
                    return info;
                case BotStateInfoType.MoveDirection:
                    info.Direction = stateInfo.Direction;
                    info.Action = null;
                    info.Walk = false;
                    return info;
                case BotStateInfoType.MoveToTarget:
                    info.Direction = MoveToTarget(stateInfo.Target);
                    info.Action = null;
                    info.Walk = false;
                    return info;
                case BotStateInfoType.Action:
                    ResetWalkDirection();
                    info.Direction = MoveToTarget(stateInfo.Target);
                    info.Action = stateInfo.AttackNow ? stateInfo.ActionData : null;
                    info.Walk = false;
                    if (stateInfo.AttackNow && stateInfo.ActionData.IsBackStep)
                    {
                        info.Direction = BackStepDirection(info.Direction);
                    }
                    return info;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float walkDirection = 0f;
        private void ResetWalkDirection()
        {
            walkDirection = 0f;
        }

        private float GetWalkDirection()
        {
            if (walkDirection == 0f)
                walkDirection = Random.Range(0, 2) * 2 - 1;
            return walkDirection;
        }


        public VectorXZ WalkAround(IEntity target)
        {
            if (target == null || target.GameObject == null)
            {
                return VectorXZ.Zero;
            }

            var targetDist = 9f;
            var diff = target.GameObject.transform.position - agent.transform.position;
            var forward = diff.normalized;
            var perpend = Quaternion.AngleAxis(90 * GetWalkDirection(), Vector3.up) * forward;
            var distDiff = diff.magnitude - targetDist;
            var move = (perpend + forward * distDiff * distDiff * Mathf.Sign(distDiff) / 5f);
            return new VectorXZ(move).Normalized;
        }

        public VectorXZ BackStepDirection(VectorXZ origDirection)
        {
            var curPos = agent.transform.position;
            var goalPos = curPos - (Vector3)origDirection * 7f;
            var possiblePos = goalPos;
            if (NavMesh.SamplePosition(goalPos, out var hit, 10f, NavMesh.AllAreas))
            {
                possiblePos = hit.position;
            }

            var targetPos = possiblePos * 2 - goalPos;
            return new VectorXZ(curPos - targetPos).Normalized;
        }

        public VectorXZ MoveToTarget(IEntity target)
        {
            if (target == null)
                return VectorXZ.Zero;
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(target.GameObject.transform.position);
                var dest = agent.path.corners[0];
                if (FromTo(agent.transform.position, dest).Magnitude < 0.1f && agent.path.corners.Length > 1)
                {
                    return ToDest(agent.path.corners[1]);
                }
                else
                {
                    return ToDest(dest);
                }
            }
            else
            {
                return ToDest(target.GameObject.transform.position);
            }
        }

        private VectorXZ FromTo(Vector3 from, Vector3 to)
        {
            return new VectorXZ(to - from);
        }

        private VectorXZ ToDest(Vector3 dest)
        {
            return FromTo(agent.transform.position, dest).Normalized;
        }
    }
}