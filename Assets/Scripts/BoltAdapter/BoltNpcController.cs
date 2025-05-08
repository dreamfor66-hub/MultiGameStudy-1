using System.Collections;
using FMLib.Structs;
using Photon.Bolt;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Bot;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Material;
using Rogue.Ingame.Network;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    public class BoltNpcController : EntityBehaviour<INpcState>, ICharacterController
    {
        [Required] public CharacterBehaviour character;
        [Required] public BotBehaviour bot;

        public int CharacterId => state.Id;
        public CharacterBehaviour Character => character;
        public bool IsMe => false;

        //for owner
        private HpInfo beforeHp = new HpInfo();

        // other
        private readonly FrameBuffer<StateFrame> stateBuffer = new FrameBuffer<StateFrame>();
        private int lastFrame = 0;

        public void Reset()
        {
            character = GetComponent<CharacterBehaviour>();
            bot = GetComponent<BotBehaviour>();
        }

        public override void Attached()
        {
            state.AddCallback("StateToken", StateTokenChanged);
            state.AddCallback("Hp", HpChanged);
            state.AddCallback("Target", TargetChanged);
            state.AddCallback("PhaseIdx", PhaseChanged);

            if (entity.IsOwner)
            {
                var id = EntityTable.GetId();
                state.Id = id;
                character.SetEntityId(id);
                character.InitServer();
            }
            else
            {
                character.SetEntityId(state.Id);
                state.AddCallback("Id", IdChanged);
            }

            character.InitNetworkState(entity.IsOwner, true, entity.IsControlled);
            PhaseChanged();
            lastFrame = BoltFrameSync.Frame - 1;
        }

        private void IdChanged()
        {
            character.SetEntityId(state.Id);
        }

        private void StateTokenChanged()
        {
            if (entity.IsOwner)
                return;

            var token = state.StateToken as CharacterStateToken;
            stateBuffer.Add(new StateFrame(TokenConverter.FromToken(token, character.CharacterData),
                token.ServerFrame));
        }

        private void HpChanged()
        {
            if (entity.IsOwner)
                return;

            var token = state.Hp as HpToken;
            var hpInfo = TokenConverter.FromToken(token);
            character.HpModule.Sync(hpInfo);
        }

        private void TargetChanged()
        {
            character.Target = EntityTable.FindById(state.Target);
        }

        private MaterialControlData curMc;
        private MaterialControlBehaviour mcBehaviour;
        private void PhaseChanged()
        {
            if (mcBehaviour == null)
                mcBehaviour = GetComponent<MaterialControlBehaviour>();
            if (curMc != null)
                mcBehaviour.Stop(curMc);
            curMc = bot.phaseData.Phases[state.PhaseIdx].MaterialData;
            if (curMc != null)
                mcBehaviour.Run(curMc);
        }

        private void FixedUpdate()
        {
            if (!entity.IsAttached)
                return;

            if (entity.IsOwner)
            {
                var control = bot.UpdateFrame();
                character.ControlAndUpdateFrame(control);
                character.BuffAccepter.UpdateFrame();
                character.HpModule.UpdateFrame();
                character.CharacterStatus.UpdateFrame();
                character.ForceAnimationUpdate();

                state.StateToken = TokenConverter.ToToken(character.TotalInfo, character.CharacterData,
                    BoltNetwork.ServerFrame);
                if (!character.HpModule.HpInfo.Equals(beforeHp))
                {
                    state.Hp = TokenConverter.ToToken(character.HpModule.HpInfo);
                    beforeHp = character.HpModule.HpInfo;
                }

                state.BuffValues.AttackSpeedPercent = character.BuffValues.AttackSpeedPercent;
                state.BuffValues.MoveSpeedPercent = character.BuffValues.MoveSpeedPercent;
                state.BuffValues.Freeze = character.BuffValues.Freeze;

                state.Target = bot.Target != null ? bot.Target.EntityId : -1;
                state.PhaseIdx = bot.PhaseIdx;
            }
            else
            {
                character.BuffValues.SyncValue(BuffSimpleValueType.AttackSpeedPercent,
                    state.BuffValues.AttackSpeedPercent);
                character.BuffValues.SyncValue(BuffSimpleValueType.MoveSpeedPercent, state.BuffValues.MoveSpeedPercent);
                character.BuffValues.SyncValue(BuffSimpleValueType.Freeze, state.BuffValues.Freeze ? 1 : 0);

                for (var frame = lastFrame + 1; frame <= BoltFrameSync.Frame; frame++)
                {
                    if (stateBuffer.FindExactly(frame, out var state))
                        character.SyncClient(state.TotalInfo.StateInfo, state.TotalInfo.Position, frame);
                    else
                        character.UpdateClientFrame();
                }
            }

            lastFrame = BoltFrameSync.Frame;
            stateBuffer.Update(lastFrame);
        }

        public void Hitstop(HitstopInfo hitstopInfo, int serverFrame)
        {
            if (!entity.IsOwner)
                return;
            character.HitstopToState(hitstopInfo);
        }

        public void SetDirection(VectorXZ direction, int serverFrame)
        {
            if (!entity.IsOwner)
                return;
            Character.SetDirectionToState(direction);
        }

        public void Hurt(HitResultInfo hitInfo, int serverFrame)
        {
            if (!entity.IsOwner)
                return;

            var knockback = hitInfo.Main.Knockback;
            if (hitInfo.Main.SuperArmor && !hitInfo.Killed)
                knockback.Strength = KnockbackStrength.JustDamage;

            character.HurtToState(knockback, hitInfo.Killed);

            if (hitInfo.Killed)
                StartCoroutine(DestroyAfter(2f));
        }

        private IEnumerator DestroyAfter(float time)
        {
            yield return new WaitForSeconds(time);
            BoltNetwork.Destroy(this.gameObject);
        }

        public void Stun(int frame, int serverFrame)
        {
            if (!entity.IsOwner)
                return;

            character.StunToState(frame);
        }

        public void ForceAction(int actionIdx, int serverFrame)
        {
            if (!entity.IsOwner)
                return;

            character.ForceActionToState(actionIdx);
        }

        public void Revive(int serverFrame)
        {
            if (!entity.IsOwner)
                return;

            character.ReviveToState();
        }

        public void AddStack(int count, int serverFrame)
        {
            if (!entity.IsOwner)
                return;

            character.AddStack(count);
        }
    }
}