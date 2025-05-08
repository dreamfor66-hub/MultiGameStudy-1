using System.Collections.Generic;
using UnityEngine;

namespace Rogue.Ingame.Camera
{
    public class CameraTarget : MonoBehaviour
    {
        public static IReadOnlyList<Transform> Targets => targets;

        private static readonly List<Transform> targets = new List<Transform>();

        private void OnEnable()
        {
            targets.Add(transform);
        }

        private void OnDisable()
        {
            if (targets.Contains(transform))
                targets.Remove(transform);
        }
    }
}