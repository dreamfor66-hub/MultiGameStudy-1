using System.Linq;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class CharacterBullet
    {
        private readonly Transform tm;
        private readonly CharacterBehaviour character;
        private readonly Team team;
        private readonly PositionAnchor[] anchors;

        private int additionalCriticalChance = 0;
        private int additionalCriticalDamagePercent = 0;

        public CharacterBullet(CharacterBehaviour character, Team team)
        {
            this.tm = character.transform;
            this.character = character;
            this.team = team;
            this.anchors = tm.GetComponentsInChildren<PositionAnchor>();
        }

        public void UpdateBullet(CharacterStateUpdateInfo info)
        {
            if (info.Cur.StateType != CharacterStateType.Action)
                return;

            var actionData = info.Cur.ActionData;
            var curFrame = info.Cur.Frame;
            var prevFrame = curFrame - info.DeltaFrame;

            foreach (var spawnData in actionData.BulletSpawnData)
            {
                var dataFrame = spawnData.Frame;
                if (prevFrame < dataFrame && dataFrame <= curFrame)
                    Spawn(spawnData, curFrame - dataFrame, actionData.AttackType);
            }

            if (actionData.BulletPatternData.PatternData != null)
            {
                foreach (var spawnData in actionData.BulletPatternData.PatternData.Bullets)
                {
                    var dataFrame = actionData.BulletPatternData.StartFrame +
                                    spawnData.Time * CommonVariables.GameFrame;
                    if (prevFrame < dataFrame && dataFrame <= curFrame)
                        Spawn(actionData.BulletPatternData.BulletPrefab2, spawnData.Position, spawnData.Angle, spawnData.AngleY,
                            spawnData.Speed, curFrame - dataFrame, actionData.AttackType);
                }
            }

            if (actionData.BulletPatternData.RandomizedPatternData != null)
            {
                foreach (var spawnData in actionData.BulletPatternData.RandomizedPatternData.Bullets)
                {
                    var dataFrame = spawnData.Frame + actionData.BulletPatternData.StartFrame;
                    if (prevFrame < dataFrame && dataFrame <= curFrame)
                        Spawn(actionData.BulletPatternData.BulletPrefab2, spawnData.Position, spawnData.Angle, spawnData.AngleY,
                            spawnData.Speed, curFrame - dataFrame, actionData.AttackType);
                }
            }
        }

        public void Spawn(BulletBehaviour prefab, Vector3 deltaPosition, float angle, float angleY, float speed, float frameOffset, ActionTypeMask actionType)
        {
            var tmPos = tm.position;
            var tmRot = tm.rotation;
            var velocity = BulletHelper.GetVelocity(angle, angleY, speed, tm.forward);
            var spawnPos = tmPos + tmRot * deltaPosition;

            GameCommandDispatcher.Send(new GameCommandSpawnBullet(prefab, spawnPos, velocity, character, team, actionType, additionalCriticalChance, additionalCriticalDamagePercent));
        }

        private void Spawn(ActionBulletSpawnData spawnData, float frameOffset, ActionTypeMask actionType)
        {
            var anchor = anchors.FirstOrDefault(x => x.Id == spawnData.AnchorId);
            var dir = GetDirection(spawnData, anchor);
            var pos = GetPosition(spawnData, anchor);
            var velocity = dir * spawnData.Speed;

            GameCommandDispatcher.Send(new GameCommandSpawnBullet(spawnData.BulletPrefab2, pos, velocity, character, team, actionType, additionalCriticalChance, additionalCriticalDamagePercent));
        }

        public void SetCritical(int criticalChance, int criticalDamagePercent)
        {
            additionalCriticalChance = criticalChance;
            additionalCriticalDamagePercent = criticalDamagePercent;
        }

        private Vector3 GetPosition(ActionBulletSpawnData spawnData, PositionAnchor anchor)
        {
            var randomX = Mathf.Abs(spawnData.RandomPosition.x);
            var randomZ = Mathf.Abs(spawnData.RandomPosition.z);
            var randomPos = new Vector3(Random.Range(-randomX, randomX), 0f, Random.Range(-randomZ, randomZ));
            if ((spawnData.AnchorUsage & PositionAnchorUsage.World) != 0)
            {
                return new Vector3(0f, tm.transform.position.y, 0f) + spawnData.Position + randomPos;
            }
            if ((spawnData.AnchorUsage & PositionAnchorUsage.TargetCharacter) != 0)
            {
                if (character.Target != null)
                    return character.Target.GameObject.transform.position + spawnData.Position;
                else
                    return new Vector3(0f, tm.transform.position.y, 0f) + spawnData.Position + randomPos;
            }
            else if ((spawnData.AnchorUsage & PositionAnchorUsage.Position) != 0 && anchor != null)
            {
                return anchor.transform.position;
            }
            else
            {
                return tm.position + tm.rotation * (spawnData.Position + randomPos);
            }
        }

        private Vector3 GetDirectionXZ(ActionBulletSpawnData spawnData, PositionAnchor anchor)
        {
            if ((spawnData.AnchorUsage & PositionAnchorUsage.World) != 0 || (spawnData.AnchorUsage & PositionAnchorUsage.TargetCharacter) != 0)
            {
                return Vector3.forward;
            }
            else if ((spawnData.AnchorUsage & PositionAnchorUsage.AngleXZ) != 0 && anchor != null)
            {
                var forward = anchor.transform.forward;
                forward.y = 0f;
                if (forward != Vector3.zero)
                {
                    return forward.normalized;
                }
            }

            {
                var forward = tm.forward;
                forward.y = 0f;
                var angle = spawnData.Angle;
                if (forward != Vector3.zero)
                    return Quaternion.AngleAxis(angle, Vector3.up) * forward.normalized;
                else
                    return Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
            }
        }

        private Vector3 GetDirection(ActionBulletSpawnData spawnData, PositionAnchor anchor)
        {
            var xz = GetDirectionXZ(spawnData, anchor);
            var angleY = Random.Range(spawnData.AngleYMin, spawnData.AngleYMax);
            if ((spawnData.AnchorUsage & PositionAnchorUsage.AngleY) != 0 && anchor != null)
            {
                angleY = Mathf.Asin(anchor.transform.forward.y) * Mathf.Rad2Deg;
            }
            var left = Vector3.Cross(xz, Vector3.up);
            return (Quaternion.AngleAxis(angleY, left) * xz);
        }
    }
}