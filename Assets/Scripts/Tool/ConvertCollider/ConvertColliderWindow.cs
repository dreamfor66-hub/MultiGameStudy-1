using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Attack;
using Rogue.Tool.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rogue.Tool.ConvertCollider
{
    public class ConvertColliderWindow : OdinEditorWindow
    {
        public GameObject GameObject;
        public ColliderListData Data;

        public List<AnchorHierarchyInfo> LoadedData = new List<AnchorHierarchyInfo>();


        [MenuItem("Tools/Rogue/Character/Convert Collider")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConvertColliderWindow>();
            window.Show();
        }

        [Button]
        public void FromGameObject()
        {
            LoadedData.Clear();
            var anchors = GameObject.GetComponentsInChildren<HitColliderAnchor>();
            LoadedData = anchors.Select(x => GetInfo(GameObject, x)).ToList();
        }

        [Button]
        public void FromData()
        {
            LoadedData.Clear();
            LoadedData = new List<AnchorHierarchyInfo>(Data.Anchors);
        }

        [Button]
        public void ToData()
        {
            Data.Anchors = new List<AnchorHierarchyInfo>(LoadedData);
            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssetIfDirty(Data);
        }

        [Button]
        public void ToGameObject()
        {
            var anchors = GameObject.GetComponentsInChildren<HitColliderAnchor>().ToList();

            foreach (var info in LoadedData)
            {
                var prevAnchor = anchors.Find(x => x.Id == info.Id);
                if (prevAnchor == null)
                {
                    CreateHitCollider(GameObject, info);
                }
                else
                {
                    ChangeAnchor(GameObject, info, prevAnchor);
                    anchors.Remove(prevAnchor);
                }
            }

            foreach (var anchor in anchors)
            {
                DestroyImmediate(anchor.gameObject);
            }

            EditorUtility.SetDirty(GameObject);
        }

        private void CreateHitCollider(GameObject root, AnchorHierarchyInfo info)
        {
            var obj = new GameObject();
            var anchor = obj.AddComponent<HitColliderAnchor>();
            SetAnchor(root, info, anchor);
        }

        private void ChangeAnchor(GameObject root, AnchorHierarchyInfo info, HitColliderAnchor anchor)
        {
            var customColliders = anchor.GetComponentsInChildren<AttackCollider>();
            foreach (var collider in customColliders)
            {
                if (collider.gameObject == anchor.gameObject)
                    DestroyImmediate(collider);
                else
                    DestroyImmediate(collider.gameObject);
            }

            SetAnchor(root, info, anchor);
        }

        private void SetAnchor(GameObject root, AnchorHierarchyInfo info, HitColliderAnchor anchor)
        {
            var parent = FindPath(root.transform, info.Path);
            var objName = $"HitCollider_{info.Id}";
            var anchorTm = anchor.transform;
            anchor.gameObject.name = objName;
            anchorTm.parent = parent;
            anchorTm.localPosition = info.TransformInfo.LocalPosition;
            anchorTm.localRotation = info.TransformInfo.LocalRotation;
            anchorTm.localScale = info.TransformInfo.LocalScale;
            anchor.Id = info.Id;
            foreach (var colliderInfo in info.Colliders)
            {
                ColliderGenerator.AddCollider(anchor.gameObject, colliderInfo);
            }
        }


        private AnchorHierarchyInfo GetInfo(GameObject root, HitColliderAnchor anchor)
        {
            var info = new AnchorHierarchyInfo();
            var tm = anchor.transform;
            info.Id = anchor.Id;
            info.Path = GetPath(root.transform, tm);
            info.TransformInfo = CreateTransformInfo(tm);
            info.Colliders = new List<ColliderTransformInfo>();

            var colliders = anchor.GetComponentsInChildren<AttackCollider>();
            info.Colliders.AddRange(colliders.Select(ColliderInfoFactory.CreateInfo).Where(x => x != null).ToList());

            return info;
        }

        private static TransformInfo CreateTransformInfo(Transform tm)
        {
            var info = new TransformInfo();
            info.LocalPosition = tm.localPosition;
            info.LocalRotation = tm.localRotation;
            info.LocalScale = tm.localScale;
            return info;
        }


        private static Transform FindPath(Transform root, string path)
        {
            var names = path.Split('/');
            var cur = root;
            foreach (var name in names)
            {
                cur = cur.Find(name);
                if (cur == null)
                {
                    Debug.LogError($"can't find path : {path}", root);
                    return null;
                }
            }
            return cur;
        }

        private static string GetPath(Transform root, Transform target)
        {
            var path = "";
            var cur = target;
            while (true)
            {
                cur = cur.parent;
                if (cur == root)
                {
                    break;
                }

                if (cur == null)
                {
                    Debug.LogError($"Get Path Error : {root.name} to {target.name}", target);
                    return "";
                }
                if (path == string.Empty)
                    path = cur.name;
                else
                    path = cur.name + "/" + path;
            }
            return path;
        }
    }
}
