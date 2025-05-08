using System;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Rogue.Tool.Timeline.ActionViewer
{
    public class ActionViewer : OdinEditorWindow
    {
        [MenuItem("Tools/Rogue/Timeline/ActionViewer")]
        public static void ShowWindow()
        {
            var window = GetWindow<ActionViewer>();
            window.Show();
        }

        protected override void OnEnable()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            LoadByCurrentScene();
        }

        protected override void OnDestroy()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            LoadBySceneName(scene.name);
        }


        [InlineEditor]
        public ActionData ActionData;
        private void LoadBySceneName(string sceneName)
        {
            if (!sceneName.StartsWith("TL_"))
            {
                ActionData = null;
                return;
            }
            var splits = sceneName.Split('_');
            var charName = splits[1];
            var idx = sceneName.IndexOf(splits[2], StringComparison.Ordinal);
            var actionName = sceneName.Substring(idx);

            var actionPath = EditorPaths.ActionDataPath(charName, actionName);

            ActionData = AssetDatabase.LoadAssetAtPath<ActionData>(actionPath);
        }

        public void LoadByCurrentScene()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            LoadBySceneName(sceneName);
        }
    }
}