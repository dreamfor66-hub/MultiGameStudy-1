using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Bolt.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rogue.Ingame.Data
{
    public class AssetTable<T> : ScriptableObject where T : Object
    {
        [FolderPath] public string[] Folders;
        public List<T> Assets = new List<T>();

        [Button]
        public void Refresh()
        {
            Assets.Clear();
#if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(ScriptableObject)))
            {
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}", Folders);
                foreach (var guid in guids)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var data = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    Assets.Add(data);
                }
            }
            else if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:prefab", Folders);
                foreach (var guid in guids)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    if (obj != null)
                        Assets.Add(obj);
                }
            }
#endif
        }

        [TitleGroup("Search"), PropertyOrder(1)]
        [ShowInInspector]
        [OnValueChanged(nameof(Search))]
        private string searchText;

        [TitleGroup("Search"), PropertyOrder(1)]
        [ShowInInspector]
        private List<T> searchResult = new List<T>();

        private void Search()
        {
            searchResult.Clear();
            var splits = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length == 0)
                return;
            foreach (var a in Assets)
            {
                if (splits.All(x => a.name.ToLower().Contains(x.ToLower())))
                    searchResult.Add(a);
            }
        }

        public bool IsValid(int id)
        {
            return 0 <= id && id < Assets.Count;
        }

        public int GetId(T obj)
        {
            var id = Assets.IndexOf(obj);
            if (obj != null && id < 0)
            {
                Debug.LogError($"can't find {obj.name} in asset table");
            }
            return id;
        }

        public T GetById(int id)
        {
            return IsValid(id) ? Assets[id] : null;
        }

    }
}