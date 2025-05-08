using UnityEngine;

namespace Rogue.Ingame.Stage
{
    public class MapCenter : MonoBehaviour
    {
        public static Transform Center;

        void Start()
        {
            Center = transform;
        }

        void OnDestroy()
        {
            if (Center == this.transform)
                Center = null;
        }

    }
}