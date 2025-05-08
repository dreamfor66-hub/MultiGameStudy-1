using System.Collections.Generic;
using System.IO;
using Photon.Bolt;
using Rogue.BoltAdapter;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Bot;
using Rogue.Ingame.Camera;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.UI;
using Rogue.Ingame.UI.Status;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

namespace Rogue.Tool.CharacterCreator
{
    public class CharacterCreatorWindow : OdinEditorWindow
    {
        public string Name;

        [Required] public Animator SkinAsset;
        [Required] public AnimationClip IdleAnimation;
        [Required] public AnimationClip RunAnimation;
        [Required] public AnimationClip DeadAnimation;
        [Required] public AnimationClip StunAnimation;
        [Space(10)]
        [Required] public AnimationClip KnockbackLowAnimation;
        [Required] public AnimationClip KnockbackMiddleAnimation;
        [Required] public AnimationClip KnockbackHighAnimation;
        [Required] public AnimationClip KnockbackStopAnimation;
        [Required] public AnimationClip GetUpMiddleAnimation;
        [Required] public AnimationClip GetUpHighAnimation;
        [Space(10)]
        [SerializeField] bool ExtraRunAnimation = false;
        [ShowIf("ExtraRunAnimation")] [Required] public AnimationClip RunAnimation_Start;
        [ShowIf("ExtraRunAnimation")] [Required] public AnimationClip RunAnimation_End;

        public CharacterCreateType PrefabCreate;

        [MenuItem("Tools/Rogue/Character/Create Character")]
        public static void ShowWindow()
        {
            var window = GetWindow<CharacterCreatorWindow>();
            window.Show();
        }

        [Button]
        public void Create()
        {
            CheckSkinAssetOptimize();
            CreateAnimatorController(Name);
            CreateModelPrefab(Name, SkinAsset);
            CreateCharacterData(Name, PrefabCreate == CharacterCreateType.Boss);

            if (PrefabCreate == CharacterCreateType.Player)
                CreatePlayerCharacter(Name);

            if (PrefabCreate == CharacterCreateType.Boss)
                CreateBossCharacter(Name);

            if (PrefabCreate == CharacterCreateType.Monster)
                CreateMonsterCharacter(Name);
        }

        [Button]
        private void CheckSkinAssetOptimize()
        {
            var path = AssetDatabase.GetAssetPath(SkinAsset.gameObject);
            var modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            modelImporter.optimizeGameObjects = false;
            modelImporter.SaveAndReimport();
        }

        private AnimatorController CreateAnimatorController(string charName)
        {
            var path = EditorPaths.AnimatorControllerPath(charName);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            var root = controller.layers[0].stateMachine;

            AddState(root, "Idle", IdleAnimation,new Vector2(300, 0));
            AddState(root, "Run", RunAnimation, new Vector2(300, 50));
            AddState(root, "Dead", DeadAnimation, new Vector2(0, 100));
            AddState(root, "Stun", StunAnimation, new Vector2(300, 100));

            if (ExtraRunAnimation)
            {
                AddState(root, "Run_Start", RunAnimation_Start, new Vector2(0, 50));
                AddState(root, "Run_End", RunAnimation_End, new Vector2(0, 0));
                ConnectState(FindStateFromName(root, "Run_Start"), FindStateFromName(root, "Run"));
                ConnectState(FindStateFromName(root, "Run_End"), FindStateFromName(root, "Idle"));
            }

            AddState(root, "KnockbackLow", KnockbackLowAnimation, new Vector2(600, 250));
            AddState(root, "KnockbackMid", KnockbackMiddleAnimation, new Vector2(600, 300));
            AddState(root, "KnockbackHigh", KnockbackHighAnimation, new Vector2(600, 350));
            AddState(root, "KnockbackStop", KnockbackStopAnimation, new Vector2(900, 250));
            AddState(root, "GetupMid", GetUpMiddleAnimation, new Vector2(900, 300));
            AddState(root, "GetupHigh", GetUpHighAnimation, new Vector2(900, 350));

            root.anyStatePosition = new Vector2(-300,0);
            root.entryPosition = new Vector2(-300,50);
            root.exitPosition = new Vector2(600,200);


            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssetIfDirty(controller);
            return controller;
        }

        private void CreateModelPrefab(string charName, Animator skinAsset)
        {
            var acPath = EditorPaths.AnimatorControllerPath(charName);
            var animController = AssetDatabase.LoadAssetAtPath<AnimatorController>(acPath);
            var obj = Instantiate(skinAsset);
            var path = EditorPaths.CharacterModelPrefabPath(charName);
            var fileName = Path.GetFileNameWithoutExtension(path);
            obj.runtimeAnimatorController = animController;
            obj.applyRootMotion = false;
            obj.name = fileName;
            var renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            var layer = PrefabCreate == CharacterCreateType.Player
                ? LayerHelper.CharacterPlayerLayer
                : LayerHelper.CharacterMonsterLayer;
            foreach (var ren in renderers)
                ren.gameObject.layer = layer;
            PrefabUtility.SaveAsPrefabAsset(obj.gameObject, path);
            DestroyImmediate(obj.gameObject);
        }

        private void AddState(AnimatorStateMachine sm, string charName, AnimationClip clip)
        {
            var state = sm.AddState(charName);
            state.motion = clip;
        }

        private void AddState(AnimatorStateMachine sm, string charName, AnimationClip clip, Vector2 pos)
        {
            var state = sm.AddState(charName, pos);
            state.motion = clip;
        }

        private void ConnectState(AnimatorState state, AnimatorState destinationState)
        {
            var transition = state.AddTransition(destinationState);
            transition.hasExitTime = true;
            transition.exitTime = 1;
            transition.hasFixedDuration = true;
            transition.duration = 0;
        }

        private AnimatorState FindStateFromName(AnimatorStateMachine sm, string charName)
        {
            foreach (var iter in sm.states)
            {
                if (iter.state.name == charName)
                    return iter.state;
            }
            Debug.LogError($"해당 이름을 가진 AnimatorState를 찾을 수 없습니다 : {charName}");
            return null;
        }

        private void CreateCharacterData(string charName, bool isBoss)
        {
            var characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.DefaultSuperArmor = isBoss;
            characterData.Hp.ImmuneInstantDeath = isBoss;
            characterData.IsBoss = isBoss;
            var path = EditorPaths.CharacterDataPath(charName);
            EditorHelper.CreateDirIfNotExist(path);
            AssetDatabase.CreateAsset(characterData, path);
        }

        private void CreatePlayerCharacter(string charName)
        {
            var path = EditorPaths.BoltPlayerCharacterPrefabPath(charName);
            var fileName = Path.GetFileNameWithoutExtension(path);

            var obj = new GameObject(fileName);
            obj.layer = LayerHelper.CharacterPlayerLayer;

            AddAnimatorObject(obj.transform, charName);
            AddCharacterComponents(obj, CharacterCreateType.Player, charName);
            AddHurtbox(obj.transform, new Vector3(0f, 1f, 0f), 0.4f, 2f);

            EditorHelper.CreateDirIfNotExist(path);
            PrefabUtility.SaveAsPrefabAsset(obj, path);
            DestroyImmediate(obj);
        }

        private void CreateMonsterCharacter(string charName)
        {
            var path = EditorPaths.BoltMonsterCharacterPrefabPath(charName);
            var fileName = Path.GetFileNameWithoutExtension(path);

            var obj = new GameObject(fileName);
            obj.layer = LayerHelper.CharacterMonsterLayer;

            CreateBotPatternData(charName);
            AddAnimatorObject(obj.transform, charName);
            AddCharacterComponents(obj, CharacterCreateType.Monster, charName);
            AddMonsterUiAnchor(obj);
            AddHurtbox(obj.transform, new Vector3(0f, 1f, 0f), 0.4f, 2f);

            EditorHelper.CreateDirIfNotExist(path);
            PrefabUtility.SaveAsPrefabAsset(obj, path);
            DestroyImmediate(obj);
        }

        private void CreateBossCharacter(string charName)
        {
            var path = EditorPaths.BoltMonsterCharacterPrefabPath(charName);
            var fileName = Path.GetFileNameWithoutExtension(path);

            var obj = new GameObject(fileName);
            obj.layer = LayerHelper.CharacterMonsterLayer;

            CreateBotPatternData(charName);
            AddAnimatorObject(obj.transform, charName);
            AddCharacterComponents(obj, CharacterCreateType.Boss, charName);
            AddBossUiAnchor(obj);
            AddHurtbox(obj.transform, new Vector3(0f, 1f, 0f), 0.4f, 2f);

            EditorHelper.CreateDirIfNotExist(path);
            PrefabUtility.SaveAsPrefabAsset(obj, path);
            DestroyImmediate(obj);
        }


        private void AddCharacterComponents(GameObject obj, CharacterCreateType type, string charName)
        {
            var characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(EditorPaths.CharacterDataPath(charName));
            var team = type == CharacterCreateType.Player ? Team.Player : Team.Monster;
            var isBoss = type == CharacterCreateType.Boss;

            var charController = obj.AddComponent<CharacterController>();
            charController.radius = .3f;
            charController.center = new Vector3(0f, 1f, 0f);
            charController.slopeLimit = 0f;
            charController.stepOffset = 0f;

            var characterBehaviour = obj.AddComponent<CharacterBehaviour>();
            characterBehaviour.characterData = characterData;
            characterBehaviour.animator = obj.GetComponentInChildren<Animator>();
            characterBehaviour.characterController = charController;
            characterBehaviour.team = team;

            var boltEntity = obj.AddComponent<BoltEntity>();

            if (type == CharacterCreateType.Player)
            {
                var boltPlayerController = obj.AddComponent<BoltPlayerController>();
                boltPlayerController.playerCharacter = characterBehaviour;

            }
            else
            {
                var botPhaseData =
                    AssetDatabase.LoadAssetAtPath<BotPhaseData>(EditorPaths.BotPhaseDataPath(charName));

                var agent = obj.AddComponent<NavMeshAgent>();
                agent.speed = 0f;
                agent.angularSpeed = 0f;
                agent.acceleration = 0f;
                agent.stoppingDistance = 0f;

                var botBehaviour = obj.AddComponent<BotBehaviour>();
                botBehaviour.character = characterBehaviour;
                botBehaviour.agent = agent;
                botBehaviour.phaseData = botPhaseData;
                botBehaviour.targetInfo.TargetableTeam = Team.Player;
                botBehaviour.targetInfo.AggroRange = 100;

                var npcController = obj.AddComponent<BoltNpcController>();
                npcController.character = characterBehaviour;
                npcController.bot = botBehaviour;

                if (type == CharacterCreateType.Boss)
                    obj.AddComponent<CameraTarget>();
            }
        }

        private void CreateBotPatternData(string charName)
        {
            var path = EditorPaths.BotPhaseDataPath(charName);
            EditorHelper.CreateDirIfNotExist(path);
            var phaseData = ScriptableObject.CreateInstance<BotPhaseData>();
            phaseData.Phases = new List<BotSinglePhaseData>();
            phaseData.Phases.Add(new BotSinglePhaseData());
            AssetDatabase.CreateAsset(phaseData, path);
        }

        private void AddAnimatorObject(Transform parent, string charName)
        {
            var modelPrefab = AssetDatabase.LoadAssetAtPath<Animator>(EditorPaths.CharacterModelPrefabPath(charName));
            var animator = PrefabUtility.InstantiatePrefab(modelPrefab, parent) as Animator;
            var animTm = animator.transform;
            animTm.localPosition = Vector3.zero;
            animTm.localRotation = Quaternion.identity;
            animTm.localScale = Vector3.one;
        }

        private void AddMonsterUiAnchor(GameObject obj)
        {
            var anchorObj = new GameObject("UIAnchor_Monster");
            anchorObj.transform.parent = obj.transform;
            var anchor = anchorObj.AddComponent<MonsterUIAnchor>();
            anchor.Reset();
        }

        private void AddBossUiAnchor(GameObject obj)
        {
            var anchorObj = new GameObject("UIAnchor_Boss");
            anchorObj.transform.parent = obj.transform;
            var anchor = anchorObj.AddComponent<BossUIAnchor>();
            anchor.Reset();
        }

        private void AddHurtbox(Transform parent, Vector3 center, float radius, float height)
        {
            var hurtboxObj = new GameObject("Hurtbox");
            hurtboxObj.layer = LayerHelper.HurtboxLayer;
            hurtboxObj.transform.parent = parent;
            hurtboxObj.transform.localPosition = Vector3.zero;
            hurtboxObj.transform.localRotation = Quaternion.identity;
            hurtboxObj.transform.localScale = Vector3.one;
            var capsuleCollider = hurtboxObj.AddComponent<CapsuleCollider>();
            capsuleCollider.isTrigger = true;
            capsuleCollider.center = center;
            capsuleCollider.radius = radius;
            capsuleCollider.height = height;
            capsuleCollider.direction = 1;
            var hurtbox = hurtboxObj.AddComponent<Hurtbox>();
            hurtbox.center = center;
        }
    }
}