using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class Character
    {
        private readonly BuffValues buffValues;
        public CharacterStateInfo StateInfo => stateMachine.Info;
        public StaminaModule StaminaModule => staminaModule;
        public StackModule StackModule => stackModule;
        public CoolTimeModule CoolTimeModule => coolTimeModule;

        private readonly CharacterStateMachine stateMachine;
        private readonly PreInputHandler<CharacterCommandDirection> preInput;
        private readonly StaminaModule staminaModule;
        private readonly StackModule stackModule;
        private readonly CoolTimeModule coolTimeModule;

        public Character(CharacterBehaviour character, BuffValues buffValues)
        {
            this.buffValues = buffValues;
            staminaModule = new StaminaModule(character.CharacterData.Stamina);
            stackModule = new StackModule(character.CharacterData.Stack);
            coolTimeModule = new CoolTimeModule();
            stateMachine = new CharacterStateMachine(character, staminaModule, stackModule, coolTimeModule);
            preInput = new PreInputHandler<CharacterCommandDirection>();
        }

        public void Hurt(KnockbackInfo knockback, bool isDie)
        {
            stateMachine.Hurt(knockback, isDie);
        }

        public void Hitstop(HitstopInfo hitstopInfo)
        {
            stateMachine.Hitstop(hitstopInfo);
        }

        public void SetDirection(VectorXZ dir)
        {
            stateMachine.SetDirection(dir);
        }

        public void Stun(int frame)
        {
            stateMachine.Stun(frame);
        }

        public void ForceAction(ActionData action)
        {
            stateMachine.ForceAction(action);
        }

        public void Revive()
        {
            stateMachine.Revive();
        }

        public void DoControl(CharacterControlInfo control, Vector3 position, Team team)
        {
            foreach (var input in control.StateCommands)
            {
                if (stateMachine.DoCommand(input, control.Direction))
                {
                    preInput.Clear();
                }
            }

            var command = new CharacterCommandDirection
            {
                Command = control.MainCommand,
                Direction = control.Direction,
            };

            if (control.MainCommand != CharacterCommandType.None)
                preInput.Push(command);
            preInput.Process(stateMachine.DoCommand);
            stateMachine.UpdateCommand(control.Direction, false);
            coolTimeModule.AfterDoCommand(stateMachine.Info);
        }

        public void DoControl(CharacterControlActionInfo control)
        {
            stateMachine.DoCommand(control.Action, control.Direction);
            stateMachine.UpdateCommand(control.Direction, control.Walk);
        }

        public void Sync(CharacterTotalInfo totalInfo, int serverFrame)
        {
            Sync(totalInfo.StateInfo, totalInfo.PreInputInfo, totalInfo.StaminaInfo, totalInfo.StackInfo, serverFrame);
        }

        public void Sync(CharacterStateInfo stateInfo, PreInputInfo preInputInfo, StaminaInfo staminaInfo,
            StackInfo stackInfo, int serverFrame)
        {
            stateMachine.Sync(stateInfo);
            staminaModule.Sync(staminaInfo);
            stackModule.Sync(stackInfo);

            var command = new CharacterCommandDirection
            {
                Command = preInputInfo.Command,
                Direction = preInputInfo.Direction,
            };
            preInput.Reset(command, preInputInfo.RemainFrame);
            coolTimeModule.Sync(stateInfo, serverFrame);
        }

        public void UpdateFrame()
        {
            var attackSpeed = PercentToMultiply(buffValues.AttackSpeedPercent);
            var moveSpeed = PercentToMultiply(buffValues.MoveSpeedPercent);
            var freeze = buffValues.Freeze;
            stateMachine.UpdateFrame(attackSpeed, moveSpeed, freeze ? 0f : 1f);
            staminaModule.UpdateFrame();
        }

        public PreInputInfo GetPreInputInfo()
        {
            var info = new PreInputInfo();
            info.Direction = preInput.Command.Direction;
            info.Command = preInput.Command.Command;
            info.RemainFrame = preInput.RemainFrame;
            return info;
        }

        private float PercentToMultiply(int percent)
        {
            return 1 + percent / 100f;
        }
    }
}