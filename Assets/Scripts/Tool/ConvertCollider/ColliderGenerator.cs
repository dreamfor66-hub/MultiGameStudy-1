using System;
using Rogue.Ingame.Attack;
using Rogue.Tool.Data;
using UnityEngine;

namespace Rogue.Tool.ConvertCollider
{


    public static class ColliderGenerator
    {
        public static void AddCollider(GameObject obj, ColliderTransformInfo info)
        {
            var colliderInfo = info.ColliderInfo;
            var transformInfo = info.TransformInfo;

            var newObj = GenerateTransform(obj, transformInfo);

            switch (colliderInfo.Type)
            {
                case ColliderType.ContinuousSphere:
                    GenerateContinuousSphere(newObj, colliderInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static GameObject GenerateTransform(GameObject parent, TransformInfo info)
        {
            var obj = new GameObject("Collider");
            var objTm = obj.transform;
            objTm.parent = parent.transform;
            objTm.localPosition = info.LocalPosition;
            objTm.localRotation = info.LocalRotation;
            objTm.localScale = info.LocalScale;
            return obj;
        }

        private static void GenerateContinuousSphere(GameObject obj, ColliderInfo info)
        {
            var continuousSphere = obj.AddComponent<ContinuousSphereCollider>();
            continuousSphere.Radius = info.Radius;
        }
    }
}