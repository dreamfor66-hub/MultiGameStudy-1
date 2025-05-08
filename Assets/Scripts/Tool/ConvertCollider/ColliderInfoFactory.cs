using Rogue.Ingame.Attack;
using Rogue.Tool.Data;
using UnityEngine;

namespace Rogue.Tool.ConvertCollider
{
    public static class ColliderInfoFactory
    {
        public static ColliderTransformInfo CreateInfo(AttackCollider collider)
        {
            var info = new ColliderTransformInfo();
            info.TransformInfo = CreateTransformInfo(collider);
            info.ColliderInfo = CreateColliderInfo(collider);
            return info;
        }

        private static TransformInfo CreateTransformInfo(AttackCollider collider)
        {
            var anchor = collider.GetComponentInParent<HitColliderAnchor>();
            var anchorTm = anchor.transform;
            var colliderTm = collider.transform;

            var info = new TransformInfo();
            if (colliderTm.parent == anchorTm)
            {
                info.LocalPosition = colliderTm.localPosition;
                info.LocalRotation = colliderTm.localRotation;
                info.LocalScale = colliderTm.localScale;
            }
            else
            {
                var mat = anchorTm.worldToLocalMatrix * colliderTm.localToWorldMatrix;
                info.LocalPosition = anchorTm.InverseTransformPoint(colliderTm.position);
                info.LocalRotation = Quaternion.Inverse(anchorTm.rotation) * colliderTm.rotation;
                info.LocalScale = mat.lossyScale;
            }

            return info;
        }

        private static ColliderInfo CreateColliderInfo(AttackCollider collider)
        {
            switch (collider)
            {
                case ContinuousSphereCollider sphere:
                    return CreateContinuousSphere(sphere);
                default:
                    Debug.LogError($"unknown type collider : {collider.GetType()}", collider);
                    return null;
            }
        }

        private static ColliderInfo CreateContinuousSphere(ContinuousSphereCollider sphere)
        {
            var info = new ColliderInfo();
            info.Type = ColliderType.ContinuousSphere;
            info.Radius = sphere.Radius;
            return info;
        }
    }
}