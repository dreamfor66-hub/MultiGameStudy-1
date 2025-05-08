using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Tool.Data
{
    public enum ColliderType
    {
        ContinuousSphere
    }

    [System.Serializable]
    public class ColliderInfo
    {
        public ColliderType Type;
        public float Radius;
    }

    [System.Serializable]
    public class TransformInfo
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
    }

    [System.Serializable]
    public class ColliderTransformInfo
    {
        public ColliderInfo ColliderInfo;
        public TransformInfo TransformInfo;
    }

    [System.Serializable]
    public class AnchorHierarchyInfo
    {
        public int Id;
        public string Path;
        public TransformInfo TransformInfo;
        public List<ColliderTransformInfo> Colliders;
    }

    [CreateAssetMenu(fileName = "new ColliderListData", menuName = "Data/ColliderList")]
    public class ColliderListData : ScriptableObject
    {
        [ReadOnly]
        public List<AnchorHierarchyInfo> Anchors;
    }
}