using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Structs;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Input;
using Rogue.Ingame.Network;
using Sirenix.OdinInspector;
using UnityEngine;
using CharacterCommand = Photon.Bolt.CharacterCommand;

namespace Rogue.BoltAdapter
{
    public interface ICharacterController
    {
        public int CharacterId { get; }
        public CharacterBehaviour Character { get; }
        public bool IsMe { get; }

        public void Hurt(HitResultInfo hitInfo, int serverFrame);
        public void Hitstop(HitstopInfo hitstopInfo, int serverFrame);
        public void SetDirection(VectorXZ direction, int serverFrame);
        public void Stun(int frame, int serverFrame);
        public void ForceAction(int actionIdx, int serverFrame);
        public void Revive(int serverFrame);
        public void AddStack(int count, int serverFrame);
    }

    public class BoltPlayerController : EntityBehaviour<IPlayerState>, ICharacterController
    {
        [Required] public CharacterBehaviour playerCharacter;
        public int CharacterId => state.Id;
        public CharacterBehaviour Character => playerCharacter;

        // entity.IsControlled 는 Owner 에게는 항상 true 인것 같다. 내 캐릭터인지 여부를 확인할때는 IsMe 를 쓸 것.
        public bool IsMe { get; private set; }
        private bool deadFlag = false;

        public override void Attached()
        {
            state.AddCallback("CurrentStateToken", CurrentStateTokenChanged);
            state.AddCallback("FixedStateToken", FixedStateTokenChanged);
            state.AddCallback("Hp", HpChanged);

            lastFixedStateInfo = new StateFrame(playerCharacter.TotalInfo, BoltNetwork.ServerFrame);
            characterUpdater = new CharacterPredictUpdater(playerCharacter, OnBeforeUpdate);

            if (entity.IsOwner)
            {
                var id = EntityTable.GetId();
                playerCharacter.SetEntityId(id);
                playerCharacter.InitServer();
                state.Id = id;
            }
            else
            {
                playerCharacter.SetEntityId(state.Id);
                state.AddCallback("Id", IdChanged);
            }

            playerCharacter.InitNetworkState(entity.IsOwner, true, false);
        }

        private void IdChanged()
        {
            playerCharacter.SetEntityId(state.Id);
        }

        public void OnBeforeUpdate(int frame)
        {
            var curEvents = evtBuffer.FindExactlyAll(frame);
            foreach (var evt in curEvents)
            {
                switch (evt)
                {
                    case HitstopFrame hitstopFrame:
                        playerCharacter.HitstopToState(hitstopFrame.HitstopInfo);
                        break;
                    case HurtFrame hurtFrame:
                        playerCharacter.HurtToState(hurtFrame.Knockback, hurtFrame.IsDie);
                        break;
                    case StunFrame stunFrame:
                        playerCharacter.StunToState(stunFrame.Frame);
                        break;
                    case ForceActionFrame forceActionFrame:
                        playerCharacter.ForceActionToState(forceActionFrame.ActionIdx);
                        break;
                    case ReviveFrame reviveFrame:
                        playerCharacter.ReviveToState();
                        break;
                    case ResetPositionFrame resetPosition:
                        playerCharacter.transform.position = resetPosition.Position;
                        break;
                    case AddStackFrame addStack:
                        playerCharacter.AddStack(addStack.Count);
                        break;
                    case SetDirectionFrame setDirection:
                        playerCharacter.SetDirectionToState(setDirection.Direction);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(evt));
                }
            }
        }

        public void ResetPosition(Transform tm)
        {
            if (!entity.IsOwner)
                return;
            evtBuffer.Add(new ResetPositionFrame(tm.position, BoltNetwork.ServerFrame + 1));
        }

        public void Hitstop(HitstopInfo hitstop, int serverFrame)
        {
            if (!entity.IsControllerOrOwner)
                return;

            evtBuffer.Add(new HitstopFrame(hitstop, serverFrame));
        }

        public void SetDirection(VectorXZ direction, int serverFrame)
        {
            if (!entity.IsControllerOrOwner)
                return;

            evtBuffer.Add(new SetDirectionFrame(direction, serverFrame));
        }

        public void Hurt(HitResultInfo hitInfo, int serverFrame)
        {
            if (!entity.IsControllerOrOwner)
                return;

            var knockback = hitInfo.Main.Knockback;
            if (hitInfo.Main.SuperArmor && !hitInfo.Killed)
                knockback.Strength = KnockbackStrength.JustDamage;

            evtBuffer.Add(new HurtFrame(knockback, serverFrame, hitInfo.Killed));
        }

        public void Stun(int frame, int serverFrame)
        {
            if (!entity.IsControllerOrOwner)
                return;

            evtBuffer.Add(new StunFrame(serverFrame, frame));
        }

        public void ForceAction(int actionIdx, int serverFrame)
        {
            if (!entity.IsControllerOrOwner)
                return;

            evtBuffer.Add(new ForceActionFrame(serverFrame, actionIdx));
        }

        public void Revive(int serverFrame)
        {
            if (!entity.IsControllerOrOwner)
                return;

            evtBuffer.Add(new ReviveFrame(serverFrame));
        }

        public void AddStack(int count, int serverFrame)
        {
            if (!entity.IsControllerOrOwner)
                return;

            evtBuffer.Add(new AddStackFrame(count, serverFrame));
        }

        private void CurrentStateTokenChanged()
        {
            if (entity.IsOwner)
                return;

            if (IsMe)
            {
            }
            else
            {
                var token = state.CurrentStateToken as CharacterStateToken;
                stateBuffer.Add(new StateFrame(TokenConverter.FromToken(token, playerCharacter.CharacterData), token.ServerFrame));
            }
        }

        private void FixedStateTokenChanged()
        {
            if (entity.IsOwner)
                return;

            if (!IsMe)
                return;

            var token = state.FixedStateToken as CharacterStateToken;
            lastFixedStateInfo = new StateFrame(TokenConverter.FromToken(token, playerCharacter.CharacterData), token.ServerFrame);

            ping = BoltFrameSync.Frame - lastFixedStateInfo.ServerFrame;
        }

        private void HpChanged()
        {
            if (entity.IsOwner)
                return;

            var token = state.Hp as HpToken;
            var hpInfo = TokenConverter.FromToken(token);
            playerCharacter.HpModule.Sync(hpInfo);
        }

        private void SetCurrent(StateFrame stateFrame)
        {
            state.CurrentStateToken = TokenConverter.ToToken(stateFrame.TotalInfo, playerCharacter.CharacterData, stateFrame.ServerFrame);
        }

        private void SetFixed(StateFrame stateFrame)
        {
            state.FixedStateToken = TokenConverter.ToToken(stateFrame.TotalInfo, playerCharacter.CharacterData, stateFrame.ServerFrame);
        }

        public override void ControlGained()
        {
            IsMe = true;
            playerCharacter.EnableCameraTarget();
            playerCharacter.gameObject.AddComponent<CharacterInteraction>();
            OwnerCharacterHolder.SetPlayer(Character);
            playerCharacter.InitNetworkState(entity.IsOwner, true, true);

            var indicator = Instantiate(CommonResources.Instance.MyCharacterIndicator, transform.position, transform.rotation);
            indicator.transform.parent = transform; // 캐릭터 scale 의 영향받지 않기 위해 
        }

        public bool ChangeCameraTarget()
        {
            var characters = EntityTable.FindEntitiesByTeam<CharacterBehaviour>(Team.Player);
            for (int i = 0; i < characters.Count; i++)
            {
                if (playerCharacter != characters[i])
                {
                    if (!characters[i].IsDead)
                    {
                        characters[i].EnableCameraTarget();
                        return true;
                    }
                }
            }
            return false;
        }

        public void DisableAllPlayerCameraTarget()
        {
            var characters = EntityTable.FindEntitiesByTeam<CharacterBehaviour>(Team.Player);
            for (int i = 0; i < characters.Count; i++)
            {
                if (!characters[i].IsDead)
                {
                    characters[i].DisableCameraTarget();
                }
            }
        }

        //for owner
        private InputState ownerCurInput;
        private readonly Queue<InputFrame> ownerInputQueue = new Queue<InputFrame>();
        private HpInfo beforeHp = new HpInfo();

        // for Controller 
        private readonly FrameBuffer<InputFrame> inputBuffer = new FrameBuffer<InputFrame>();

        // owner and Controller
        private CharacterPredictUpdater characterUpdater;
        private StateFrame lastFixedStateInfo;
        private readonly FrameBuffer<IDataFrame> evtBuffer = new FrameBuffer<IDataFrame>();
        private readonly int maxFixedTermFrame = 12;

        // other
        private readonly FrameBuffer<StateFrame> stateBuffer = new FrameBuffer<StateFrame>();
        private int lastFrame = 0;
        private int ping = 0;

        private void FixedUpdate()
        {
            //하단 IsControlled와 다르게 Server/Client 구분X
            if (IsMe)
            {
                if (playerCharacter.IsDead && !deadFlag)
                {
                    if (ChangeCameraTarget())
                        playerCharacter.DisableCameraTarget();
                    deadFlag = true;
                }
                else if (!playerCharacter.IsDead && deadFlag)
                {
                    DisableAllPlayerCameraTarget();
                    playerCharacter.EnableCameraTarget();
                    deadFlag = false;
                }
            }

            if (entity.IsOwner)
            {
                evtBuffer.Update(lastFixedStateInfo.ServerFrame);
                characterUpdater.Reset(lastFixedStateInfo, ownerCurInput);

                if (ownerInputQueue.Count > 0)
                {
                    var inputDelay = (BoltNetwork.ServerFrame - maxFixedTermFrame) - ownerInputQueue.Last().RealFrame;
                    inputDelay = Mathf.Clamp(inputDelay, 0, 60);
                    state.InputDelayFrame = inputDelay;
                }

                while (ownerInputQueue.Count > 0)
                {
                    var input = ownerInputQueue.Dequeue();
                    if (input.ServerFrame <= characterUpdater.Frame)
                    {
                        ownerCurInput = input.Input;
                        continue;
                    }

                    characterUpdater.UpdateUntil(input.ServerFrame - 1, ownerCurInput);
                    characterUpdater.Update(input.Input);
                    ownerCurInput = input.Input;
                }

                characterUpdater.UpdateUntil(BoltNetwork.ServerFrame - maxFixedTermFrame, ownerCurInput);
                lastFixedStateInfo = characterUpdater.GetStateFrame();
                SetFixed(lastFixedStateInfo);

                characterUpdater.UpdateUntil(BoltNetwork.ServerFrame, ownerCurInput);
                playerCharacter.PredictEnd();
                playerCharacter.BuffAccepter.UpdateFrame();
                playerCharacter.HpModule.UpdateFrame();
                playerCharacter.CharacterStatus.UpdateFrame();
                playerCharacter.ForceAnimationUpdate();
                SetCurrent(characterUpdater.GetStateFrame());

                if (!playerCharacter.HpModule.HpInfo.Equals(beforeHp))
                {
                    state.Hp = TokenConverter.ToToken(playerCharacter.HpModule.HpInfo);
                    beforeHp = playerCharacter.HpModule.HpInfo;
                }

                state.BuffValues.AttackSpeedPercent = playerCharacter.BuffValues.AttackSpeedPercent;
                state.BuffValues.MoveSpeedPercent = playerCharacter.BuffValues.MoveSpeedPercent;
                state.BuffValues.Freeze = playerCharacter.BuffValues.Freeze;

            }
            else if (IsMe)
            {
                playerCharacter.BuffValues.SyncValue(BuffSimpleValueType.AttackSpeedPercent, state.BuffValues.AttackSpeedPercent);
                playerCharacter.BuffValues.SyncValue(BuffSimpleValueType.MoveSpeedPercent, state.BuffValues.MoveSpeedPercent);
                playerCharacter.BuffValues.SyncValue(BuffSimpleValueType.Freeze, state.BuffValues.Freeze ? 1 : 0);

                var curFrame = lastFixedStateInfo.ServerFrame;
                inputBuffer.Update(curFrame);
                if (inputBuffer.FindRecent(curFrame, out var curInput))
                {
                    characterUpdater.Reset(lastFixedStateInfo, curInput.Input);
                    characterUpdater.UpdateUntil(BoltFrameSync.Frame - maxFixedTermFrame, curInput.Input);
                    while (characterUpdater.Frame < BoltFrameSync.Frame)
                    {
                        inputBuffer.FindRecent(characterUpdater.Frame + 1, out var nextInput);
                        characterUpdater.Update(nextInput.Input);
                    }
                }

                playerCharacter.PredictEnd();
            }
            else
            {
                playerCharacter.BuffValues.SyncValue(BuffSimpleValueType.AttackSpeedPercent, state.BuffValues.AttackSpeedPercent);
                playerCharacter.BuffValues.SyncValue(BuffSimpleValueType.MoveSpeedPercent, state.BuffValues.MoveSpeedPercent);
                playerCharacter.BuffValues.SyncValue(BuffSimpleValueType.Freeze, state.BuffValues.Freeze ? 1 : 0);

                for (var frame = lastFrame + 1; frame <= BoltFrameSync.Frame; frame++)
                {
                    if (stateBuffer.FindExactly(frame, out var state))
                        playerCharacter.SyncClient(state.TotalInfo.StateInfo, state.TotalInfo.Position, frame);
                    else
                        playerCharacter.UpdateClientFrame();
                }

                lastFrame = BoltFrameSync.Frame;
                stateBuffer.Update(lastFrame);
            }
        }

        private int lastInputFrame;
        public override void SimulateController()
        {
            var input = InputLock.IsLocked ? new InputState() : InputDetector.GetState();
            var serverFrame = entity.IsOwner ? BoltNetwork.ServerFrame : BoltFrameSync.Frame;

            if (lastInputFrame >= serverFrame)
                return;
            lastInputFrame = serverFrame;
            var inputDelay = state.InputDelayFrame;

            ICharacterCommandInput command = CharacterCommand.Create();
            command.Buttons = input.Buttons;
            command.Direction = input.Direction;
            command.ServerFrame = serverFrame;
            command.InputDelay = inputDelay;
            entity.QueueInput(command);
            inputBuffer.Add(new InputFrame(input, serverFrame, inputDelay));
        }

        private void OnGUI()
        {
            var curSession = BoltMatchmaking.CurrentSession;
            if (curSession != null)
            {
                var str = $"{curSession.HostName}";
                GUI.color = Color.green;
                GUI.Label(new Rect(0, 0, 300, 100), str);
                GUI.color = Color.white;
            }

            if (!entity.IsOwner && IsMe)
            {
                var curFrame = BoltFrameSync.Frame;
                var fixedFrame = lastFixedStateInfo.ServerFrame;
                var diff = curFrame - fixedFrame;
                var str = $"ping = {ping}, diff = {diff} ({curFrame} - {fixedFrame}";
                if (diff > 6)
                    GUI.color = Color.red;
                else if (diff > 4)
                    GUI.color = Color.yellow;

                GUI.Label(new Rect(0, 20, 300, 100), str);
                GUI.color = Color.white;
            }
        }

        private int lastInputServerFrame;

        public override void ExecuteCommand(Command command, bool resetState)
        {
            if (!entity.IsOwner)
                return;

            var cmd = command as CharacterCommand;
            ownerInputQueue.Enqueue(new InputFrame(new InputState(cmd.Input.Direction, cmd.Input.Buttons), cmd.Input.ServerFrame, cmd.Input.InputDelay));
            lastInputServerFrame = cmd.ServerFrame;
        }
    }
}
