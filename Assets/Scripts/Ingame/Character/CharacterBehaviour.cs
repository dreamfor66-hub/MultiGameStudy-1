using System.Collections.Generic;
using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Character.CharacterImpl;
using Rogue.Ingame.Core;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.EntityMessage;
using Rogue.Ingame.Camera;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class CharacterBehaviour : MonoBehaviour, IEntity
    {
        public static IReadOnlyList<CharacterBehaviour> Characters => characters;
        private static readonly List<CharacterBehaviour> characters = new List<CharacterBehaviour>();
        public bool IsDead => Character.StateInfo.StateType == CharacterStateType.Dead;

        public int EntityId { get; private set; }
        public IEntity Target { get; set; }
        public GameObject GameObject => gameObject;
        public EntityMessageDispatcher MessageDispatcher { get; } = new EntityMessageDispatcher();

        public CharacterTotalInfo TotalInfo => new CharacterTotalInfo
        {
            StateInfo = Character.StateInfo,
            PreInputInfo = Character.GetPreInputInfo(),
            StaminaInfo = Character.StaminaModule.Info,
            StackInfo = Character.StackModule.Info,
            Position = new VectorXZ(transform.position),
        };

        [SerializeField][Required] public CharacterData characterData;
        [SerializeField][Required] public Animator animator;
        [SerializeField][Required] public CharacterController characterController;
        [SerializeField] public Team team;

        public BuffValues BuffValues => buffValues;
        public BuffAccepter BuffAccepter => buffAccepter;
        public BuffSync BuffSync => buffSync;
        public HpModule HpModule => hpModule;
        public CharacterStatus CharacterStatus => characterStatus;
        public StaminaModule StaminaModule => Character.StaminaModule;
        public StackModule StackModule => Character.StackModule;
        public CharacterBullet CharacterBullet => characterBullet;
        public CharacterAttack CharacterAttack => characterAttack;

        public CharacterMaterial CharacterMaterial => characterMaterial;

        public Team Team => team;
        public bool Hide => buffValues.Hide;
        public CameraTarget cameraTarget = null;
        public Character Character
        {
            get;
            private set;
        }

        public CharacterData CharacterData => characterData;

        private BuffValues buffValues;
        private HpModule hpModule;
        private BuffCondition buffCondition;
        private BuffAccepter buffAccepter;
        private BuffSync buffSync;
        private BuffVfx buffVfx;

        private CharacterMove characterMove;
        private CharacterAnimator characterAnimator;
        private CharacterVfx characterVfx;
        private CharacterAttack characterAttack;
        private CharacterStatus characterStatus;
        private CharacterSpawn characterSpawn;
        private CharacterBullet characterBullet;
        private CharacterBuff characterBuff;
        private CharacterMaterial characterMaterial;
        private CharacterImplement[] characterImpls;
        private CharacterStateInfo prevStateInfo;

        private void Awake()
        {
            buffValues = new BuffValues();
            hpModule = new HpModule(characterData.Hp, buffValues);
            buffCondition = new BuffCondition(hpModule, characterData.IsBoss);
            buffAccepter = new BuffAccepter(buffCondition, buffValues, this);
            buffSync = new BuffSync();
            buffVfx = new BuffVfx(this.transform, buffSync);

            Character = new Character(this, buffValues);
            characterMove = new CharacterMove(characterData, characterController, this);
            characterAnimator = new CharacterAnimator(characterData, animator);
            characterVfx = new CharacterVfx(transform);
            characterAttack = new CharacterAttack(transform, characterData, this);
            characterStatus = new CharacterStatus(gameObject);
            characterSpawn = new CharacterSpawn(gameObject);
            characterBullet = new CharacterBullet(this, team);
            characterBuff = new CharacterBuff(this);
            characterMaterial = new CharacterMaterial(this);
            prevStateInfo = Character.StateInfo;

            characters.Add(this);

            var hurtboxes = GetComponentsInChildren<Hurtbox>();
            foreach (var hurtbox in hurtboxes)
            {
                hurtbox.SetEntity(this);
                hurtbox.SetDefaultSuperArmor(characterData.DefaultSuperArmor);
            }

            characterImpls = GetComponentsInChildren<CharacterImplement>();

            EntityTable.Add(this);
            AttackCalculator.Add(characterAttack);
            UpdateStateInfo(false);
        }


        private void OnDestroy()
        {
            hpModule.OnDestroy();
            buffAccepter.Clear();
            characterVfx.OnDestroy();
            characters.Remove(this);
            EntityTable.Remove(this);
            AttackCalculator.Remove(characterAttack);
        }

        public void InitServer()
        {
            foreach (var buff in characterData.Buff.StartBuffs)
                buffAccepter.AddBuff(buff, this);
        }

        public void InitNetworkState(bool logic, bool visual, bool controller)
        {
            characterImpls.ForEach(x => x.InitNetworkState(logic, visual, controller));
        }

        public void SetEntityId(int id)
        {
            EntityId = id;
        }

        public void SyncOnly(CharacterTotalInfo totalInfo, int frame)
        {
            Character.Sync(totalInfo, frame);
            var tm = transform;
            tm.position = new Vector3(totalInfo.Position.x, tm.position.y, totalInfo.Position.z);
        }

        public void HurtToState(KnockbackInfo knockback, bool isDie)
        {
            Character.Hurt(knockback, isDie);
        }

        public void HitstopToState(HitstopInfo hitstopInfo)
        {
            Character.Hitstop(hitstopInfo);
        }

        public void SetDirectionToState(VectorXZ dir)
        {
            Character.SetDirection(dir);
        }

        public void StunToState(int frame)
        {
            Character.Stun(frame);
        }

        public void GainStatus(CharacterStatusType type, int frame)
        {
            characterStatus.AddToModule(type, frame);
        }

        public void ForceActionToState(int actionIdx)
        {
            if (actionIdx < 0 || actionIdx >= characterData.PossibleActions.Count)
                return;

            var actionData = CharacterData.PossibleActions[actionIdx];
            Character.ForceAction(actionData);
        }

        public void ReviveToState()
        {
            Character.Revive();
        }

        public void AddStack(int count)
        {
            StackModule.Add(count);
        }

        public void HurtToEntity(HitResultInfo hitResult)
        {
            MessageDispatcher.Send(new EntityMessageHurt(hitResult));
        }

        public void HitToEntity(HitResultInfo hitResult)
        {
            MessageDispatcher.Send(new EntityMessageHit(hitResult));
        }

        public void AggroToEntity(IEntity target, float value)
        {
            MessageDispatcher.Send(new EntityMessageAggro(target, value));
        }

        public void PredictUpdateFrame(CharacterControlInfo controlInfo)
        {
            var prev = Character.StateInfo;
            Character.DoControl(controlInfo, transform.position, team);
            Character.UpdateFrame();
            var update = new CharacterStateUpdateInfo(prev, Character.StateInfo);
            characterMove.UpdateMove(update, controlInfo.Direction);
            characterImpls.ForEach(x => x.UpdateMove(update));
        }

        public void ForceAnimationUpdate()
        {
            animator.Update(0);
            characterAttack.MemoryPosition();
        }

        public void PredictEnd()
        {
            UpdateStateInfo(false);
        }

        public void SyncClient(CharacterStateInfo info, VectorXZ position, int frame)
        {
            Character.Sync(info, default, default, default, frame);
            UpdateStateInfo(false);
            var tm = transform;
            tm.position = new Vector3(position.x, tm.position.y, position.z);
        }

        public void ControlAndUpdateFrame(CharacterControlActionInfo controlInfo)
        {
            Character.DoControl(controlInfo);
            UpdateClientFrame();
        }

        public void UpdateClientFrame()
        {
            Character.UpdateFrame();
            UpdateStateInfo(true);
        }

        private void UpdateStateInfo(bool move)
        {
            var cur = Character.StateInfo;
            var updateInfo = new CharacterStateUpdateInfo(prevStateInfo, cur);
            if (move)
            {
                characterMove.UpdateMove(updateInfo, VectorXZ.Zero);
                characterImpls.ForEach(x => x.UpdateMove(updateInfo));
            }

            characterAnimator.UpdateAnimator(updateInfo);
            characterVfx.UpdateVfx(updateInfo);
            characterAttack.UpdateAttack(updateInfo);
            characterStatus.UpdateStatus(updateInfo);
            characterBullet.UpdateBullet(updateInfo);
            characterSpawn.UpdateSpawn(updateInfo);
            characterBuff.UpdateBuff(updateInfo);
            characterImpls.ForEach(x => x.UpdateStateInfo(updateInfo));
            prevStateInfo = cur;
        }

        public void EnableCameraTarget()
        {
            if (cameraTarget == null)
                cameraTarget = gameObject.AddComponent<CameraTarget>();
            cameraTarget.enabled = true;
        }

        public void DisableCameraTarget()
        {
            if (cameraTarget != null)
                cameraTarget.enabled = false;
        }
    }
}