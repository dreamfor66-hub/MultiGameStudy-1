using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine.Timeline;
using Rogue.Ingame.Data;
using Rogue.Tool;
using UnityEditor.SceneManagement;
using UnityEngine.Playables;
using System.Linq;

namespace Rogue
{
    public class TimelineAttributer : OdinEditorWindow
    {

        //public static string[] Categories => AssetDatabase.GetSubFolders(EditorPaths.TimelineSceneRootPath);

        [ValueDropdown("GetTimeline")]
        public string Category;

        string CharacterName;
        string ActionName;
        enum EditType   
        {
            Rename,
            Duplicate,
            Delete
        }
        [SerializeField] EditType CurEditType;
        [HideIf(nameof(CurEditType),EditType.Delete)]
        [SerializeField] string changedActionName;
        [Space(10)]
        [DisplayAsString] [SerializeField] string CurSelectedName = "";
        [HideIf(nameof(CurEditType), EditType.Delete)] [DisplayAsString] [SerializeField] string FileNameChangeTo = "";
        TimelineAsset changedTimelineAsset = null;
        List<TrackAsset> TimelineTrackList = new List<TrackAsset>();

        string AnimatorPath => EditorPaths.AnimatorControllerPath(CharacterName);
        string TimelineAssetPath => EditorPaths.TimelineAssetPath(CharacterName, ActionName);
        string TimelineScenePath => EditorPaths.TimelineScenePath(CharacterName, ActionName);
        string ActionDataPath => EditorPaths.ActionDataPath(CharacterName, ActionName);
        string changedTimelineAssetPath => EditorPaths.TimelineAssetPath(CharacterName, changedActionName);
        string changedTimelineScenePath => EditorPaths.TimelineScenePath(CharacterName, changedActionName);
        string changedActionDataPath => EditorPaths.ActionDataPath(CharacterName, changedActionName);

        IEnumerable GetTimeline()
        {
            return AssetDatabase.FindAssets("t:TimelineAsset").Select(x => AssetDatabase.GUIDToAssetPath(x)).Select(x => new ValueDropdownItem(x, x.Split('/')[3].Split('.')[0]));
        }

        [MenuItem("Tools/Rogue/Timeline/TimelineAttributer")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimelineAttributer>();
            window.Show();
        }

        [ShowIf(nameof(CurEditType), EditType.Rename)]
        [Button(Name = "Change Asset Name")]
        void ChangeTimelineName()
        {
            if (EditorHelper.IsAssetExist(TimelineAssetPath))
                AssetDatabase.RenameAsset(TimelineAssetPath, $"TL_{CharacterName}_{changedActionName}");
            else
                LogErrorEmptyFile(TimelineAssetPath);
            if (EditorHelper.IsAssetExist(TimelineScenePath))
                AssetDatabase.RenameAsset(TimelineScenePath, $"TL_{CharacterName}_{changedActionName}");
            else
                LogErrorEmptyFile(TimelineScenePath);
            if (EditorHelper.IsAssetExist(ActionDataPath))
            {
                var actionData = AssetDatabase.LoadAssetAtPath<ActionData>(ActionDataPath);
                actionData.ActionKey = changedActionName;
                EditorUtility.SetDirty(actionData);
                AssetDatabase.RenameAsset(ActionDataPath, $"Action_{CharacterName}_{changedActionName}");
            }
            else
                LogErrorEmptyFile(ActionDataPath);

            if (EditorHelper.IsAssetExist(AnimatorPath))
            {
                var animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorPath);
                for (int i = 0; i < animator.layers[0].stateMachine.states.Length; i++)
                {
                    if (animator.layers[0].stateMachine.states[i].state.name == ActionName)
                    {
                        animator.layers[0].stateMachine.states[i].state.name = changedActionName;
                        break;
                    }
                }
            }
            else
                LogErrorEmptyFile(AnimatorPath);

            AssetDatabase.SaveAssets();
        }
        [ShowIf(nameof(CurEditType), EditType.Duplicate)]
        [Button(Name = "Duplicate Asset")]
        void DuplicateTimeline()
        {
            changedTimelineAsset = null;
            if (EditorHelper.IsAssetExist(TimelineAssetPath))
            {
                AssetDatabase.CopyAsset(TimelineAssetPath, changedTimelineAssetPath);
                AssetDatabase.SaveAssets();
                changedTimelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(changedTimelineAssetPath) ?? null;
                EditorUtility.SetDirty(changedTimelineAsset);
            }
            else
                LogErrorEmptyFile(TimelineAssetPath);
            if (EditorHelper.IsAssetExist(ActionDataPath))
            {
                AssetDatabase.CopyAsset(ActionDataPath, changedActionDataPath);
                var actionData = AssetDatabase.LoadAssetAtPath<ActionData>(changedActionDataPath);
                actionData.ActionKey = changedActionName;
                EditorUtility.SetDirty(actionData);
            }
            else
                LogErrorEmptyFile(ActionDataPath);
            if (EditorHelper.IsAssetExist(TimelineScenePath))
            {
                EditorSceneManager.OpenScene(TimelineScenePath);
                var beforePlayableDirector = FindObjectOfType<PlayableDirector>() ?? null;
                TimelineTrackList.Clear();
                TimelineTrackList.AddRange((beforePlayableDirector.playableAsset as TimelineAsset).GetOutputTracks().ToList());
                AssetDatabase.CopyAsset(TimelineScenePath, changedTimelineScenePath);

                var scene = EditorSceneManager.OpenScene(changedTimelineScenePath);

                var playerDirector = FindObjectOfType<PlayableDirector>() ?? null;
                playerDirector.playableAsset = changedTimelineAsset;
                var NewTimelineTrackList = playerDirector.playableAsset.outputs.ToList();
                for (int i = 0; i < TimelineTrackList.Count; i++)
                {
                    playerDirector.SetGenericBinding(NewTimelineTrackList[i].sourceObject, playerDirector.GetGenericBinding(TimelineTrackList[i]));// 기존 저장된 경로에 맞춰 새로운 트랙에 적용
                    playerDirector.ClearGenericBinding(TimelineTrackList[i]);// 기존 파일 제거
                }

                EditorSceneManager.SaveScene(scene);
            }
            else
                LogErrorEmptyFile(TimelineScenePath);
            AssetDatabase.SaveAssets();
        }

        [ShowIf(nameof(CurEditType), EditType.Delete)]
        [Button(Name = "Delete Asset")]
        public void DeleteTimeline()
        {
            if (EditorHelper.IsAssetExist(TimelineAssetPath))
                AssetDatabase.DeleteAsset(TimelineAssetPath);
            else
                LogErrorEmptyFile(TimelineAssetPath);
            if (EditorHelper.IsAssetExist(TimelineScenePath))
                AssetDatabase.DeleteAsset(TimelineScenePath);
            else
                LogErrorEmptyFile(TimelineScenePath);
            if (EditorHelper.IsAssetExist(ActionDataPath))
                AssetDatabase.DeleteAsset(ActionDataPath);
            else
                LogErrorEmptyFile(ActionDataPath);

            if (EditorHelper.IsAssetExist(AnimatorPath))
            {
                var animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorPath);
                for (int i = 0; i < animator.layers[0].stateMachine.states.Length; i++)
                {
                    if (animator.layers[0].stateMachine.states[i].state.name == ActionName)
                    {
                        animator.layers[0].stateMachine.RemoveState(animator.layers[0].stateMachine.states[i].state);
                        break;
                    }
                }
            }
            else
                LogErrorEmptyFile(AnimatorPath);
            AssetDatabase.Refresh();
        }

        private void Update()
        {
            if (!string.IsNullOrEmpty(Category) && Category.Contains('_'))
            {
                string MixedActionName = "";
                var SplittedText = Category.Split('_');
                CharacterName = SplittedText[1];
                for (int i = 2; i < SplittedText.Length; i++)
                {
                    if (i != 2)
                        MixedActionName += "_";
                    MixedActionName += SplittedText[i];
                }
                ActionName = MixedActionName;
            }

            CurSelectedName = $"TL_{CharacterName}_{ActionName}";
            FileNameChangeTo = $"TL_{CharacterName}_{changedActionName}";

        }

        void LogErrorEmptyFile(string path)
        {
            Debug.LogError($"<color=red>해당 파일이 존재하지 않습니다 : </color>{path}");
        }
    }
}
