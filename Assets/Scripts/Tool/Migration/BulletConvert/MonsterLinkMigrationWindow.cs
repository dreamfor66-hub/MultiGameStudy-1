using System.Collections.Generic;
using System.IO;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Rogue.Tool.Migration.BulletConvert
{
    public struct MonsterUpgrade
    {
        public GameObject Prev;
        public GameObject Now;
    }


    public class MonsterLinkMigrationWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Rogue/Migration/Monster Link")]
        public static void ShowWindow()
        {
            GetWindow<MonsterLinkMigrationWindow>().Show();
        }

        public List<MonsterUpgrade> Changes = new List<MonsterUpgrade>();



        public StageRunner stageRunner;

        public GameObject Find(GameObject legacy)
        {
            foreach (var change in Changes)
            {
                if (change.Prev == legacy)
                    return change.Now;
            }

            return null;
        }

        [Button]
        public void StageMigration()
        {
            foreach (var wave in stageRunner.waves)
            {
                foreach (var monster in wave.Monsters)
                {
                    // monster.MonsterPrefab = Find(monster.MonsterPrefabLegacy != null ? monster.MonsterPrefabLegacy.gameObject : null);
                    // monster.MonsterPrefabLegacy = null;
                }
            }
            EditorUtility.SetDirty(stageRunner);

        }



        [Button]
        public void FindChanges()
        {
            Changes.Clear();

            var prevPath = "Assets/Prefabs/Characters/Monsters";
            var prevGuids = AssetDatabase.FindAssets("", new string[] { prevPath });
            var toPrev = new Dictionary<string, GameObject>();
            foreach (var guid in prevGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(path);
                var keyword = FileNameToKeyword(fileName);
                toPrev.Add(keyword, AssetDatabase.LoadAssetAtPath<GameObject>(path));
            }

            toPrev["Spook"] = null;

            var nowPath = "Assets/Prefabs/Bolt/Monster";
            var nowGuids = AssetDatabase.FindAssets("", new string[] { nowPath });
            foreach (var guid in nowGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(path);
                var keyword = FileNameToKeyword(fileName);
                if (toPrev.ContainsKey(keyword))
                {
                    Changes.Add(new MonsterUpgrade
                    {
                        Prev = toPrev[keyword],
                        Now = AssetDatabase.LoadAssetAtPath<GameObject>(path)
                    });
                }
                else
                {
                    Debug.LogError($"can't find legacy for : {path}");
                }
            }
        }

        public string FileNameToKeyword(string fileName)
        {
            var splits = fileName.Split('_');
            var keyword = "";
            foreach (var split in splits)
            {
                if (split == "PF")
                    continue;
                if (split == "Monster")
                    continue;
                if (split == "Bolt")
                    continue;
                keyword += split;
            }
            return keyword;
        }



    }
}