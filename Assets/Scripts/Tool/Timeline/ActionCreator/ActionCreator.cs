using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.ActionCreator
{
    [System.Serializable]
    public class TimelineTrackOptions
    {
        public bool CreateHitColliderTrackGroup = true;
        public bool CreateVfxTrackGroup = true;
    }

    public class ActionCreator : OdinEditorWindow
    {
        [MenuItem("Tools/Rogue/Timeline/ActionCreator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ActionCreator>();
            window.Show();
        }

        [AssetsOnly]
        public GameObject CharacterPrefab;
        public string ActionName;
        public AnimationClip AnimationClip;
        public TimelineTrackOptions Options;

        private string CharacterName => CharacterPrefab.name.Split('_')[1];

        [Button]
        public void Create()
        {
            ValidateArguments();
            var scene = CreateTimelineScene();
            var characterObj = CreateCharacterObject();
            var timelineAsset = CreateTimelineAsset();
            var playableDirector = TimelineBinder.BindTimeline(characterObj, timelineAsset);
            EditorHelper.SaveForce(scene);
            EditorHelper.SaveForce(timelineAsset);
            EditorHelper.FocusTimeline(playableDirector);
            ActionDataCreator.CreateActionDataIfNotExist(CharacterName, ActionName);
        }

        private void ValidateArguments()
        {
            if (CharacterPrefab == null)
                throw new ArgumentException("Empty Character Prefab");
            if (string.IsNullOrEmpty(ActionName))
                throw new ArgumentException("Empty Action Name");
            if (AnimationClip == null)
                throw new ArgumentException("Empty Animation Clip");
        }

        private Scene CreateTimelineScene()
        {
            var path = EditorPaths.TimelineScenePath(CharacterName, ActionName);
            return TimelineSceneCreator.CreateTimelineSceneIfNotExist(path);
        }

        private GameObject CreateCharacterObject()
        {
            return TimelineSceneCreator.CreateCharacterObj(CharacterPrefab);
        }


        private TimelineAsset CreateTimelineAsset()
        {
            var path = EditorPaths.TimelineAssetPath(CharacterName, ActionName);
            return TimelineAssetCreator.CreateTimelineAsset(path, AnimationClip, Options);
        }
    }
}