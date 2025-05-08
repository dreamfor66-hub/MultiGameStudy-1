using Rogue.Ingame.Data;
using UnityEditor;
using UnityEngine;

namespace Rogue.Tool.Timeline.ActionCreator
{
    public static class ActionDataCreator
    {
        public static void CreateActionDataIfNotExist(string characterName, string actionName)
        {
            var actionPath = EditorPaths.ActionDataPath(characterName, actionName);

            if (!EditorHelper.IsAssetExist(actionPath))
            {
                EditorHelper.CreateDirIfNotExist(actionPath);
                var actionData = ScriptableObject.CreateInstance<ActionData>();
                actionData.ActionKey = actionName;
                AssetDatabase.CreateAsset(actionData, actionPath);
                Debug.Log($"Action Data Created : {actionPath}");
            }
            else
            {
                Debug.Log($"Action Data Already Exist : {actionPath}");
            }
        }
    }
}