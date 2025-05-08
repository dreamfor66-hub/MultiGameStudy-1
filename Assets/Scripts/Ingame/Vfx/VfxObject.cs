using System;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Vfx
{
    public enum VfxType
    {
        Time,
        Manual,
        Loop,
    }

    public enum FollowType
    {
        None,
        Position,
        PositionAndRotation
    }

    [RequireComponent(typeof(ParticleSystem))]
    public class VfxObject : MonoBehaviour, ISpawnable
    {
        public ParticleSystem Particle;

        public VfxType VfxType;
        public FollowType FollowType;

        [ShowIf(nameof(VfxType), VfxType.Time)]
        public float DespawnTime = 1f;

        private Transform followTm;
        private Vector3 localPosition;
        private Quaternion localRotation;

        private Action despawnAction;

        public void RegisterDespawn(Action action)
        {
            despawnAction = action;
        }

        private float elapsedTime;
        public void OnSpawn()
        {
            elapsedTime = 0f;
            if (VfxType == VfxType.Manual)
            {
                Particle.Pause(true);
            }
            else
            {
                Particle.Play(true);
            }
        }

        public void OnDespawn()
        {
            followTm = null;
        }

        public void SetFollow(Transform tm, Vector3 localPos, Quaternion localRot, Vector3 localScale)
        {
            followTm = tm;
            localPosition = localPos;
            localRotation = localRot;

            var position = tm.rotation * localPos + tm.position;
            var rotation = tm.rotation * localRot;

            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = localScale;
        }

        private void Despawn()
        {
            despawnAction?.Invoke();
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (VfxType == VfxType.Time)
            {
                if (elapsedTime > DespawnTime)
                {
                    Despawn();
                }
            }
        }

        private void LateUpdate()
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (FollowType == FollowType.None)
                return;
            if (followTm == null)
                return;

            if (FollowType == FollowType.Position)
            {
                var position = followTm.rotation * localPosition + followTm.position;
                transform.position = position;
            }
            else if (FollowType == FollowType.PositionAndRotation)
            {
                var position = followTm.TransformPoint(localPosition);
                var rotation = followTm.rotation * localRotation;
                transform.SetPositionAndRotation(position, rotation);
            }
        }

        public void SetTime(float time)
        {
            if (VfxType != VfxType.Manual)
            {
                Debug.LogError($"must be Manual Type : {gameObject.name}", this);
            }
            Particle.Simulate(time, true);
        }

        public void DespawnManually()
        {
            if (VfxType == VfxType.Time)
            {
                Debug.LogError($"must be Manual or Loop Type : {gameObject.name}", this);
            }

            Despawn();
        }

        public void Reset()
        {
            Particle = GetComponent<ParticleSystem>();
        }
    }
}
