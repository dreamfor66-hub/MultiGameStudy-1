using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Rogue
{
    public class RigController : MonoBehaviour
    {
        [SerializeField] private Rig rig;
        [SerializeField] private Transform targetTransform;
        float rigWeight = 0;

        public bool targetTracking
        {
            get;
            private set;
        } = false;
        private GameObject targetObject;
        private Vector3 trackingOffset;

        void FixedUpdate()
        {
            if (targetTracking)
            {
                targetTransform.transform.position = targetObject.transform.position + trackingOffset;
            }
            rig.weight = rigWeight;
        }

        public void EnableRig(GameObject targetObject, Vector3 trackingOffset)
        {
            this.targetTracking = true;
            this.targetObject = targetObject;
            this.trackingOffset = trackingOffset;
            SetRigWeight(1);
        }

        public void DisableRig()
        {
            targetTracking = false;
            SetRigWeight(0);
        }

        public void SetRigWeight(float weight)
        {
            rigWeight = weight;
        }
    }
}
