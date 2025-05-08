using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Rogue.Tool
{
    public static class EditorHelper
    {
        public static bool IsAssetExist(string path)
        {
            return !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path));
        }

        public static void FocusTimeline(PlayableDirector playableDirector)
        {
            var window = TimelineEditor.GetOrCreateWindow();
            window.SetTimeline(playableDirector);
            window.locked = true;
        }

        public static void SaveForce(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
        }

        public static void SaveForce(Scene scene)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        public static void CreateDirIfNotExist(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static string GetUniqueFileDirectory(string path, string extension)
        {
            CreateDirIfNotExist(path);
            string meshPathWithExtension = $"{path}.{extension}";
            // 기존 파일이 있으면 넘버링을 붙여주며 올라간다.
            int duplicatedNum = 1;
            while (File.Exists(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + meshPathWithExtension))
            {
                duplicatedNum++;
                meshPathWithExtension = $"{path}{duplicatedNum}.{extension}";
            }
            return meshPathWithExtension;
        }
    }
}