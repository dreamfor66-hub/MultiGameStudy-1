using System;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    public class SpawnArea : MonoBehaviour
    {
        public Vector3 Min;
        public Vector3 Max;

        public void OnDrawGizmos()
        {
            var center = (Min + Max) / 2f;
            var size = Max - Min;
            Gizmos.color = Color.green;
            Gizmos.DrawCube(center + new Vector3(0f, 0.01f, 0f), size);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center + new Vector3(0f, 0.01f, 0f), size + Vector3.one * 0.5f);
        }
    }
}