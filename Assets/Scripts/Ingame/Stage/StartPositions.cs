using System.Collections.Generic;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    public class StartPositions : MonoBehaviour
    {
        public List<Transform> Positions;

        private int idx = 0;

        public Transform GetTransform()
        {
            return Positions[idx++ % Positions.Count];
        }
    }
}