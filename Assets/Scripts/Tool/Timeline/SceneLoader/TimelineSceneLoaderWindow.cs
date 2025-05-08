using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace Rogue.Tool.Timeline.SceneLoader
{
    public class TimelineSceneLoaderWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Rogue/Timeline/SceneLoader")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimelineSceneLoaderWindow>();
            window.Show();
        }

        public static string[] Categories => AssetDatabase.GetSubFolders(EditorPaths.TimelineSceneRootPath);

        [ValueDropdown(nameof(Categories))]
        public string Category;

        [Button]
        public void GameBase()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/GameBase.unity");
        }

        [OnInspectorGUI]
        public void DrawButtons()
        {
            if (string.IsNullOrEmpty(Category))
                return;
            var guids = AssetDatabase.FindAssets("t:Scene", new string[] { Category });
            var names = guids.Select(AssetDatabase.GUIDToAssetPath).Select(Path.GetFileNameWithoutExtension);
            foreach (var name in names)
            {
                if (GUILayout.Button(name, GUILayout.Height(30f)))
                {
                    LoadSceneAndSelectTimeline(Category, name);
                }
            }
        }

        private static void LoadSceneAndSelectTimeline(string category, string name)
        {
            var path = $"{category}/{name}.unity";
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                var scene = EditorSceneManager.OpenScene(path);
                var director = FindObjectOfType<PlayableDirector>();
                EditorHelper.FocusTimeline(director);
            }
        }
    }
}