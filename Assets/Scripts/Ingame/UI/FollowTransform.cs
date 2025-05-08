using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class FollowTransform : MonoBehaviour
    {
        public Transform FollowTm;
        public Vector3 Offset;

        private void LateUpdate()
        {
            if (FollowTm != null)
                transform.position = FollowTm.transform.position + Offset;
        }
    }
}