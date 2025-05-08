using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SDG.Lib.Util
{
    [Serializable]
    [HideLabel]
    public class SceneRef
    {
        [HideInInspector] public string SceneName;

#if UNITY_EDITOR
        [OnValueChanged(nameof(OnSceneChanged))]
        [HideLabel]
        public UnityEditor.SceneAsset Scene;
        private void OnSceneChanged()
        {
            SceneName = Scene != null ? Scene.name : "";
        }
#endif
    }
}
