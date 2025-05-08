using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;
using Object = UnityEngine.Object;

namespace Rogue.Tool.ReferenceFinder
{

    [Serializable]
    public class AssetFindOption
    {
        public bool CheckPrefab = true;
        public bool CheckScene = true;
        public bool CheckScriptableObject = true;
        public bool CheckAnimationClip = false;
        public bool CheckAnimatorController = false;
        public bool CheckMaterial = false;
        public bool CheckModel = false;
    }

    public class FindResult
    {
        public Object obj;
        public string reference;
    }

    public class ReferenceFinderWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Rogue/Reference/Reference Finder")]
        public static void ShowWindow()
        {
            GetWindow<ReferenceFinderWindow>().Show();
        }

        [FolderPath]
        public string[] RootPaths = { "Assets" };

        public AssetFindOption Option;

        [AssetsOnly]
        public Object[] Objs;

        [TableList]
        public List<FindResult> Results = new List<FindResult>();

        [Button]
        public void Find()
        {
            try
            {
                Results.Clear();
                var guids = new List<string>();
                var guidTable = new Dictionary<string, string>();
                foreach (var obj in Objs)
                {
                    var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogError($"can't find guid : {obj.name}", obj);
                        continue;
                    }
                    guids.Add(guid);
                    guidTable.Add(guid, obj.name);
                }

                var paths = AllPaths();

                for (int i = 0; i < paths.Length; i++)
                {
                    var path = paths[i];
                    if (path.EndsWith(".fbx") || path.EndsWith(".FBX"))
                        path = $"{path}.meta";

                    if (EditorUtility.DisplayCancelableProgressBar($"{i}/{paths.Length}", path,
                        (float)i / paths.Length))
                    {
                        break;
                    }

                    var s = File.ReadAllText(path);
                    var resultStr = "";
                    foreach (var guid in guids)
                    {
                        if (s.Contains(guid))
                            resultStr += $"{guidTable[guid]}, ";
                    }

                    if (!resultStr.IsNullOrEmpty())
                    {
                        Results.Add(new FindResult
                        {
                            obj = AssetDatabase.LoadAssetAtPath<Object>(path),
                            reference = resultStr,
                        });
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private string[] AllPaths()
        {
            var filter = "";
            if (Option.CheckPrefab)
                filter += "t:Prefab ";
            if (Option.CheckScene)
                filter += "t:Scene ";
            if (Option.CheckScriptableObject)
                filter += "t:ScriptableObject ";
            if (Option.CheckAnimatorController)
                filter += "t:AnimatorController ";
            if (Option.CheckAnimationClip)
                filter += "t:AnimationClip ";
            if (Option.CheckMaterial)
                filter += "t:Material ";
            if (Option.CheckModel)
                filter += "t:Model ";

            return AssetDatabase.FindAssets(filter, RootPaths).Select(AssetDatabase.GUIDToAssetPath).ToArray();
        }
    }
}
