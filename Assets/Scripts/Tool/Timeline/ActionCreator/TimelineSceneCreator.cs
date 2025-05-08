using System;
using System.IO;
using Rogue.Ingame.Camera;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Rogue.Tool.Timeline.ActionCreator
{
    public static class TimelineSceneCreator
    {
        public static Scene CreateTimelineSceneIfNotExist(string path)
        {
            if (EditorHelper.IsAssetExist(path))
            {
                throw new ArgumentException($"Timeline Scene Already Exist : {path}");
            }
            return CreateScene(path);
        }

        private static Scene CreateScene(string path)
        {
            EditorHelper.CreateDirIfNotExist(path);
            var scene = EditorSceneManager.OpenScene(EditorPaths.TimelineTemplateScenePath);
            EditorSceneManager.SaveScene(scene, path, false);
            return scene;
        }

        public static GameObject CreateCharacterObj(GameObject characterPrefab)
        {
            var characterObj = PrefabUtility.InstantiatePrefab(characterPrefab) as GameObject;
            return characterObj;
        }
    }
}