using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Data;
using System.Linq;
using Rogue.Ingame.Data.Buff;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Character.CharacterImpl
{
    public class ArcherCharacterImplement : CharacterImplement
    {
        private bool logic;
        private bool visual;
        private bool controller;
        private bool isInit = false;

        private bool isREPressed;
        private bool isRE2Pressed;
        private bool hasWire => wireBullet != null;
        private bool hasSpecialCheck => characterBehaviour.BuffAccepter.HasBuff(BuffTag.RobinSpecialCheck);

        private const string WireAction1 = "SpecialAttack_remake_RE";
        private const string WireAction2 = "SpecialAttack_remake_RE2";

        [SerializeField] private CharacterController characterController;
        [SerializeField] private CharacterBehaviour characterBehaviour;
        [SerializeField] private Transform chargeRangeUI;
        [SerializeField] private RobinCameraExtension robinCameraExtension;
        [SerializeField] private RobinChargeBar ChargeBarUI;
        [SerializeField] private BulletData WireBulletData;
        [SerializeField] private LineRenderer WireLineRenderer;
        [SerializeField, BoxGroup("LineRenderer")] private Transform WireBodyTarget;
        [SerializeField, BoxGroup("LineRenderer")] private Vector3 WireBodyTargetOffset;
        [SerializeField, BoxGroup("LineRenderer")] private Vector3 WireBulletTargetOffset;

        private BulletBehaviour wireBullet;

        private Vector3 lastCameraPos;

        public void Awake()
        {
            chargeRangeUI.gameObject.SetActive(false);
            SetSpecialBuffRemover();
            SetWireRemover();
        }

        private void OnEnable()
        {
            ChargeBarUI.InitChargeBarSetting();
        }

        private void OnDestroy()
        {
            RemoveAllMyWireTarget();
        }
        public override void InitNetworkState(bool logic, bool visual, bool controller)
        {
            this.logic = logic;
            this.visual = visual;
            this.controller = controller;
            isInit = true;
        }

        private void SetSpecialBuffRemover()
        {
            SceneManager.activeSceneChanged += SpecialBuffRemove;
        }

        private void SetWireRemover()
        {
            // logic이 아니면 씬이 넘어갈때 Buff를 지워주면서 WireTarget을 없애주지 못하기에 이렇게라도 처리해준다.
            SceneManager.activeSceneChanged += (Scene scene1, Scene scene2) => RemoveWireTarget();
        }

        void SpecialBuffRemove(Scene scene, Scene scene2)
        {
            if (characterBehaviour == null || characterBehaviour.BuffAccepter == null)
                return;
            if (characterBehaviour.BuffAccepter.GetBuffs().Count <= 0)
                return;
            var buffs = characterBehaviour.BuffAccepter.GetBuffs();
            var taggedBuffs = buffs.Where(x => (x.Data?.Tags.Count > 0));
            var buff = taggedBuffs.FirstOrDefault(x => x.Data.Tags.Contains(BuffTag.RobinSpecial));
            if (buff != null)
                characterBehaviour.BuffAccepter.RemoveBuff(buff.Data);
        }

        public override void UpdateStateInfo(CharacterStateUpdateInfo updateInfo)
        {
            if (controller)
            {
                UpdateChargeRange(updateInfo);
                UpdateChargeVarValue(updateInfo);
                UpdateChargeRangeCameraTarget(updateInfo);
            }

            SetWireActionInteraction(updateInfo);
            DrawWireLine();
        }

        private Vector3 moveXStartPos;
        private Vector3 moveXTargetPos;
        private bool targetEnabled;
        public override void UpdateMove(CharacterStateUpdateInfo updateInfo)
        {
            if (updateInfo.Cur.ActionData == null)
                return;

            if (updateInfo.Cur.ActionData.ActionKey.Contains(WireAction2))
            {
                if (!targetEnabled && hasWire)
                {
                    targetEnabled = true;
                    moveXStartPos = transform.position;
                    moveXTargetPos = wireBullet.transform.position;
                    moveXTargetPos += (moveXTargetPos - moveXStartPos).normalized * 1.5f;
                    Debug.Log($"Target : {moveXTargetPos:0.00}, Start : { moveXStartPos:0.00}");
                }

                if (targetEnabled)
                {
                    var movePower = 2.5f;
                    var moveStartframe = 10;
                    var moveEndFrame = 70;
                    var moveFrame = moveEndFrame - moveStartframe;
                    var curFrame = updateInfo.Cur.Frame;

                    var curRemain = Mathf.Clamp(moveEndFrame - curFrame, 0, moveFrame);
                    var diff = moveXTargetPos - moveXStartPos;
                    var coEff = diff.magnitude / Mathf.Pow(moveFrame, movePower);
                    var dir = diff.normalized;
                    var curPos = coEff * Mathf.Pow(curRemain, movePower) * (-dir) + moveXTargetPos;
                    MoveTo(curPos);
                }

                if (hasWire)
                {
                    var diff = wireBullet.transform.position - transform.position;
                    diff.y = 0f;
                    if (diff != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(diff, Vector3.up);
                }
            }
            else if (wireBullet == null)
            {
                targetEnabled = false;
            }
        }

        private void MoveTo(Vector3 target)
        {
            var tmPos = characterController.transform.position;
            var delta = target - tmPos;
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                delta = hit.position - tmPos;
            }
            characterController.Move(delta);
        }

        void SetWireActionInteraction(CharacterStateUpdateInfo updateInfo)
        {
            if (updateInfo.Cur.ActionData != null)
            {
                if (updateInfo.Cur.ActionData.ActionKey == WireAction1 && !isREPressed && ((logic && !hasSpecialCheck) || !logic))
                {
                    isREPressed = true;
                    CheckWireisEnable();
                    if (!hasWire)
                        StartCoroutine(FindMyTarget(0.2f, 12));
                }

                if (updateInfo.Cur.ActionData.ActionKey == WireAction2 && !isRE2Pressed)
                {
                    isRE2Pressed = true;
                    FindMyWireTarget();
                }

                if (updateInfo.Cur.ActionData.ActionKey.Contains(WireAction2))
                {
                    var afterDelayFrame = 20f;
                    if (updateInfo.Cur.Frame >= updateInfo.Cur.ActionData.TotalFrame - afterDelayFrame || characterBehaviour.Target == null || CloseToTarget(1f))
                    {
                        RemoveWireTarget();
                    }
                }

                if (updateInfo.Cur.ActionData.ActionKey != WireAction1)
                {
                    isREPressed = false;
                }
                else if (updateInfo.Cur.ActionData.ActionKey != WireAction2)
                {
                    isRE2Pressed = false;
                }
            }
            else
            {
                isREPressed = false;
                isRE2Pressed = false;

                if (characterBehaviour.IsDead ||wireBullet == null || (logic && !characterBehaviour.BuffAccepter.HasBuff(BuffTag.RobinSpecial)))
                    RemoveWireTarget();
            }
        }

        bool CloseToTarget(float MinDistance)
        {
            if (characterBehaviour.Target == null)
                return true;

            return Vector3.Distance(characterBehaviour.Target.GameObject.transform.position, characterBehaviour.GameObject.transform.position) <= MinDistance;
        }

        void UpdateChargeVarValue(CharacterStateUpdateInfo updateInfo)
        {
            if (!string.IsNullOrEmpty(updateInfo.Cur.ActionData?.ActionKey))
            {
                ChargeBarUI.SetValueAutomacally(updateInfo.Cur.ActionData.ActionKey, (int)updateInfo.Cur.Frame);
            }
        }

        public bool SetWireTarget()
        {
            if (characterBehaviour.Target == null)
            {
                wireBullet = FindMyWireTarget() ?? null;
                if (hasWire)
                {
                    characterBehaviour.Target = wireBullet;
                    return true;
                }
            }
            return false;
        }

        public BulletBehaviour FindMyWireTarget()
        {
            var list = EntityTable.FindByType<BulletBehaviour>().Where(x => (x.Info.RootSourceId == characterBehaviour.EntityId) && x.bulletData == WireBulletData).ToList();
            while (list.Count > 1)
            {
                list[list.Count - 1].End();
                list.RemoveAt(list.Count - 1);
            }
            return list.Count > 0 ? list[0] : null;
        }

        public void RemoveAllMyWireTarget()
        {
            var list = EntityTable.FindByType<BulletBehaviour>().Where(x => (x.Info.RootSourceId == characterBehaviour.EntityId) && x.bulletData == WireBulletData).ToList();
            while (list.Count > 0)
            {
                if (list[list.Count - 1] != null)
                    list[list.Count - 1].End();
                list.RemoveAt(list.Count - 1);
            }
        }

        private void CheckWireisEnable()
        {
            if (wireBullet != null)
            {
                if (!wireBullet.gameObject.activeSelf)
                {
                    RemoveWireTarget();
                }
            }
            else
            {
                characterBehaviour.Target = null;
            }
        }

        private void RemoveWireTarget()
        {
            if (hasWire)
                wireBullet.End();
            wireBullet = null;
            characterBehaviour.Target = null;
        }

        private void DrawWireLine()
        {
            if (hasWire)
            {
                WireLineRenderer.positionCount = 2;
                WireLineRenderer.SetPosition(0, WireBodyTarget.position + WireBodyTargetOffset);
                WireLineRenderer.SetPosition(1, wireBullet.transform.position + WireBulletTargetOffset);
            }
            else
            {
                WireLineRenderer.positionCount = 0;
            }
        }

        private void UpdateChargeRangeCameraTarget(CharacterStateUpdateInfo updateInfo)
        {
            var curState = updateInfo.Cur;
            var prevState = updateInfo.Prev;
            float startValue = -0.5f;
            float allValue = 1f;
            float linearSpeed = 0.08f;
            float curValue = 0;

            if (curState.ActionData != null)
            {
                var actionKey = curState.ActionData.ActionKey;
                var prevActionKey = prevState.ActionData != null ? updateInfo.Prev.ActionData?.ActionKey : null;
                if (actionKey == "BasicAttack_Down_RE")
                {
                    curValue = Mathf.Clamp(curState.Frame * linearSpeed, 0, allValue);
                }
                else if (actionKey.StartsWith("BasicAttack_Down"))
                {
                    curValue = allValue;
                }

                if ((actionKey.Contains("Down") || actionKey.Contains("start")) && (prevState.ActionData == null || !prevActionKey.Contains("Down")))
                {
                    robinCameraExtension.SetTargetPositionForce(transform.position);
                }
                else if (actionKey.Contains("Up") && (prevState.ActionData == null || !prevActionKey.Contains("Up")))
                {
                    lastCameraPos = transform.position + transform.forward * 15;
                    robinCameraExtension.FreezeInHere(lastCameraPos, 0.666f);
                }
                else if (actionKey.Contains("Down"))
                {
                    robinCameraExtension.SetTargetPosition(transform.position + transform.forward * (curValue * 15));
                }

                if (actionKey.Contains("BasicAttack"))
                {
                    robinCameraExtension.TargetEnable(true);
                }
            }
            else
            {
                if (!robinCameraExtension.isFreezed && robinCameraExtension.CameraEnable)
                {
                    robinCameraExtension.TargetEnable(false);
                    robinCameraExtension.SetTargetPositionForce(transform.position);
                }
            }
        }

        private void UpdateChargeRange(CharacterStateUpdateInfo updateInfo)
        {
            var curState = updateInfo.Cur;
            if (IsEnableChargeUI(curState))
            {
                chargeRangeUI.gameObject.SetActive(true);
                ChargeBarUI.gameObject.SetActive(true);
            }
            else
            {
                chargeRangeUI.gameObject.SetActive(false);
                ChargeBarUI.gameObject.SetActive(false);
            }
        }

        private bool IsEnableChargeUI(CharacterStateInfo stateInfo)
        {
            if (stateInfo.StateType != CharacterStateType.Action)
                return false;
            var actionKey = stateInfo.ActionData.ActionKey;
            return actionKey.StartsWith("BasicAttack_Down");
        }

        private bool IsEnableWireAction(CharacterStateInfo stateInfo)
        {
            if (stateInfo.StateType != CharacterStateType.Action)
                return false;
            var actionKey = stateInfo.ActionData.ActionKey;
            return actionKey.StartsWith("SpecialAttack_remake_RE");
        }

        private IEnumerator FindMyTarget(float delay, int Count)
        {
            if (Count <= 0)
                yield return null;

            if (!SetWireTarget())
            {
                yield return new WaitForSeconds(delay);
                StartCoroutine(FindMyTarget(delay, Count - 1));
            }
        }
    }
}
