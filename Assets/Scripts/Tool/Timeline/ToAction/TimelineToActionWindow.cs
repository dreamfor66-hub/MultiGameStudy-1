using System;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Rogue.Tool.Timeline.ToAction
{
    [HideLabel]
    [Serializable]
    public class TimelineToActionInfo
    {
        [Title("From")]
        public PlayableDirector PlayableDirector;
        public string AnimationKey;

        [Title("To")]
        public ActionData ActionData;
        public AnimatorController Controller;
    }

    public class TimelineToActionWindow : OdinEditorWindow
    {
        public TimelineToActionInfo Info;

        [MenuItem("Tools/Rogue/Timeline/ToActionData")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimelineToActionWindow>();
            window.Show();
            window.AutoLink();
        }

        protected override void OnEnable()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            AutoLink();
        }

        protected override void OnDestroy()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            AutoLink();
        }



        [Title("Buttons")]
        [Button]
        public void AutoLink()
        {
            Info = TimelineToActionAutoLinker.AutoLink();
        }

        [Button]
        public void ToActionData()
        {
            var converter = new TimelineToActionConverter(Info);
            converter.Convert();
        }
    }
}