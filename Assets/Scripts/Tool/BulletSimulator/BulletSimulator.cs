using Rogue.Ingame.Bullet;
using UnityEngine;

namespace Rogue.Tool.BulletSimulator
{
    public class BulletSimulator : MonoBehaviour
    {
        public BulletBehaviour Prefab;
        public float Period;
        public Vector3 Position;
        public Vector3 Velocity;
        public float RotationSpeed;

        private float time;

        private void Update()
        {
            UpdateSpawn();
        }

        private void UpdateSpawn()
        {
            time += Time.deltaTime;
            if (time > Period)
            {
                time -= Period;
                Spawn();
            }
        }

        private void Spawn()
        {
            //BulletSpawner.Spawn(Prefab, Position, Velocity, RotationSpeed, null, null, ActionTypeMask.None);
        }
    }
}
