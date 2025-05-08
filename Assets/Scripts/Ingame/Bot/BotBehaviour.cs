using System;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Rogue.Ingame.Bot
{
    [HideLabel]
    [Title("Target")]
    [Serializable]
    public struct BotTargetInfo
    {
        public Team TargetableTeam;
        public float AggroRange;
        public bool UseCustomAggroVariables;

        [ShowIf(nameof(UseCustomAggroVariables), true)]
        public AggroVariables CustomAggroVariables;
    }


    [RequireComponent(typeof(CharacterBehaviour))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class BotBehaviour : MonoBehaviour
    {
        [SerializeField] [Required] public CharacterBehaviour character;
        [SerializeField] [Required] public NavMeshAgent agent;
        [SerializeField] public RigController RigController;

        [SerializeField][Required][Title("Pattern")] public BotPhaseData phaseData;
        [SerializeField] public bool UseCustomScoreVariables;
        [SerializeField][ShowIf(nameof(UseCustomScoreVariables), true)] public BotPatternScoreVariables CustomScoreVariables;
        [SerializeField] public BotTargetInfo targetInfo;

        public IEntity Target => patternAI.Target;

        private BotController controller;
        private BotPatternAi patternAI;

        public bool DebugAggro = false;
        public bool isFreezed = false;
        public int PhaseIdx => patternAI.PhaseIdx;


        private void Awake()
        {
            controller = new BotController(agent);
            patternAI = new BotPatternAi(character,
                                        targetInfo,
                                        phaseData,
                                        targetInfo.UseCustomAggroVariables ? targetInfo.CustomAggroVariables : null,
                                        UseCustomScoreVariables ? CustomScoreVariables : null,
                                        isFreezed);
        }

        private void OnDestroy()
        {
            patternAI.OnDestroy();
        }

        public CharacterControlActionInfo UpdateFrame()
        {
            patternAI.UpdateState();
            var info = patternAI.GetInfo();
            UpdateRig(info);
            return controller.StateToControl(info);
        }

        private void Reset()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<CharacterBehaviour>();
        }

        private GUIStyle style = new GUIStyle();
        private void OnGUI()
        {
            if (DebugAggro)
            {
                style.fontSize = 24;
                style.normal.textColor = Color.white;
                GUI.Label(new Rect(0, 80, 1200, 800), patternAI.TargetModel.AggroTable.GetDebugString(), style);
                GUI.Label(new Rect(0, 240, 1200, 800), patternAI.ScoreModel.GetDebugString(), style);
            }
        }

        void UpdateRig(BotStateInfo info)
        {
            if (!RigController)
                return;

            if (info.StateType == BotStateInfoType.WalkAround)
            {
                if (!RigController.targetTracking)
                    if (Target != null)
                        RigController.EnableRig(Target.GameObject, new Vector3(0, 1f, 0));
            }
            else
            {
                RigController.DisableRig();
            }
        }
    }
}
