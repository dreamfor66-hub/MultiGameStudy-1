using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rogue.Ingame.Character;
using Rogue.Ingame.Camera;

namespace Rogue
{
    public class RobinCameraExtension : MonoBehaviour
    {
        [SerializeField] CameraTarget cameraTarget;
        private Vector3 curPosition => cameraTarget.transform.position;
        private Vector3 TargetPosition;
        public bool isFreezed
        {
            get;
            private set;
        }
        public bool CameraEnable => cameraTarget.enabled;
        [SerializeField] private float MoveSpeed;
        bool isReached;
        CharacterBehaviour characterBehaviour;
        Coroutine FreezeCoroutine;

        void Start()
        {
            TargetEnable(false);
        }

        void FixedUpdate()
        {
            if (cameraTarget.enabled)
            {
                if (!isReached)
                    MoveCameraTarget();
            }
        }

        void MoveCameraTarget()
        {
            if (isFreezed || (TargetPosition - curPosition).magnitude < MoveSpeed)
            {
                cameraTarget.transform.position = TargetPosition;
                if (!isFreezed)
                    isReached = true;
            }
            else
            {
                cameraTarget.transform.position += (TargetPosition - curPosition).normalized * MoveSpeed;
            }
        }

        public void SetTargetPositionForce(Vector3 target)
        {
            StopFreezeCoroutine();
            cameraTarget.transform.position = target;
            isReached = true;
        }

        public void SetTargetPosition(Vector3 target)
        {
            if (!isFreezed)
            {
                TargetPosition = target;
                isReached = false;
            }
        }

        public void TargetEnable(bool enable)
        {
            cameraTarget.enabled = enable;
        }

        public void FreezeInHere(Vector3 target, float delay)
        {
            StopFreezeCoroutine();
            FreezeCoroutine = StartCoroutine(SetTargetPositionCoroutine(target, delay));
        }

        IEnumerator SetTargetPositionCoroutine(Vector3 target, float delay)
        {
            SetTargetPosition(target);
            isFreezed = true;
            yield return new WaitForSeconds(delay);
            isFreezed = false;
        }

        public void StopFreezeCoroutine()
        {
            if (FreezeCoroutine != null)
            {
                StopCoroutine(FreezeCoroutine);
                isFreezed = false;
            }
        }
    }
}
