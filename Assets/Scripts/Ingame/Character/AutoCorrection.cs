using System.Linq;
using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public static class AutoCorrection
    {
        public static VectorXZ CorrectedDirection(Vector3 position, VectorXZ direction, Team team, float maxDistance, float defaultDistance, float AutoCorrectionMaxAngle, float AutoCorrectionMinAngle)
        {
            var targetPositions = EntityTable.Entities.OfType<CharacterBehaviour>()
                .Where(x => AttackTeamHelper.Attackable(team, x.team) && !x.IsDead).Select(x => x.transform.position);

            var selectedScore = float.MaxValue;
            var selectedPosition = position + direction;

            foreach (var pos in targetPositions)
            {
                var diff = pos - position;
                var dist = diff.magnitude;
                var angle = Vector3.Angle(diff, direction);

                if (dist > maxDistance)
                    continue;

                var distRatio = maxDistance < defaultDistance ? 0f :
                                dist - defaultDistance / (maxDistance -
                                            defaultDistance);
                distRatio = Mathf.Clamp01(distRatio);

                var angleMax = Mathf.Lerp(AutoCorrectionMaxAngle,
                    AutoCorrectionMinAngle, distRatio);

                if (angle > angleMax)
                    continue;

                var otherFocal = position + ((Vector3)direction).normalized * maxDistance / 2f;
                var score = dist + Vector3.Distance(otherFocal, pos);
                if (score < selectedScore)
                {
                    selectedScore = score;
                    selectedPosition = pos;
                }
            }

            return new VectorXZ(selectedPosition - position).Normalized;
        }
    }
}