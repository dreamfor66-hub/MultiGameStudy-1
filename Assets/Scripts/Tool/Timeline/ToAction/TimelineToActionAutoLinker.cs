using System;
using Rogue.Ingame.Data;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Rogue.Tool.Timeline.ToAction
{
    public static class TimelineToActionAutoLinker
    {
        public static TimelineToActionInfo AutoLink()
        {
            var info = new TimelineToActionInfo();

            info.PlayableDirector = Object.FindObjectOfType<PlayableDirector>();

            var sceneName = SceneManager.GetActiveScene().name;
            if (!sceneName.StartsWith("TL"))
                return info;

            var splits = sceneName.Split('_');
            var charName = splits[1];

            var idx = sceneName.IndexOf(splits[2], StringComparison.Ordinal);
            var actionName = sceneName.Substring(idx);

            info.AnimationKey = actionName;
            var actionPath = EditorPaths.ActionDataPath(charName, actionName);
            var animControllerPath = EditorPaths.AnimatorControllerPath(charName);

            info.ActionData = AssetDatabase.LoadAssetAtPath<ActionData>(actionPath);
            info.Controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(animControllerPath);

            return info;
        }
    }
}