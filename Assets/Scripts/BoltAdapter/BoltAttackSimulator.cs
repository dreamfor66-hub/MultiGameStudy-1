using System.Collections.Generic;
using FMLib.Structs;
using Photon.Bolt;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Core;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using Rogue.Ingame.Input;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class BoltAttackSimulator : GlobalEventListener
    {
        private List<HitResultInfo> hitResults = new List<HitResultInfo>();
        private List<HealResultInfo> healResults = new List<HealResultInfo>();
        private List<IGameCommand> commands = new List<IGameCommand>();

        public void FixedUpdate()
        {
            AttackCalculator.CastAll(ref hitResults);
            BoltSpawnEventServer.Instance.SpawnAll();
            BuffCalculator.CalculateAll();
            HealCalculator.GetResult(ref healResults);
            OtherCommandBuffer.GetResult(ref commands);

            foreach (var result in hitResults)
            {
                CreateHitEvent(result).Send();
            }

            foreach (var result in healResults)
            {
                CreateHealEvent(result).Send();
            }

            foreach (var command in commands)
            {
                if (command is GameCommandStun stun)
                    CreateStunEvent(stun).Send();
                if (command is GameCommandParryingSuccess parrying)
                    CreateParryingSuccessEvent(parrying.ParryingInfo).Send();
                if (command is GameCommandForceAction forceAction)
                    CreateForceActionEvent(forceAction).Send();
                if (command is GameCommandRevive revive)
                    CreateReviveEvent(revive).Send();
                if (command is GameCommandAddStack addStack)
                    CreateAddStackEvent(addStack).Send();
                if (command is GameCommandGainStatus gainStatus)
                {
                    if (gainStatus.Target is CharacterBehaviour character)
                        character.GainStatus(gainStatus.Type, gainStatus.Frame);
                }
                if (command is GameCommandGainRootAggro gainAggro)
                {
                    if (gainAggro.Defender is CharacterBehaviour character)
                        character.AggroToEntity(gainAggro.Attacker, gainAggro.Value);
                }
            }
        }

        public override void OnEvent(RequestReviveEvent evnt)
        {
            var target = EntityTable.FindById(evnt.TargetId);
            if (target is CharacterBehaviour character)
            {
                if (BoltDungeonManager.Instance.ReviveCount > 0 && character.IsDead)
                {
                    GameCommandRevive.Send(target, 50);
                    BoltDungeonManager.Instance.DoRevive();
                }
            }
        }

        private HitEvent CreateHitEvent(HitResultInfo hit)
        {
            var hitEvent = HitEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            hitEvent.Attacker = hit.Main.Attacker?.EntityId ?? -1;
            hitEvent.AttackerRoot = hit.Main.AttackerRoot?.EntityId ?? -1;
            hitEvent.Victim = hit.Main.Victim?.EntityId ?? -1;
            hitEvent.HitFxId = HitFxTable.Instance.GetId(hit.Main.HitFx);
            hitEvent.KnockbackDirection = DirectionEncoder.ToAngle(new VectorXZ(hit.Main.Knockback.Direction));
            hitEvent.KnockbackDistance = hit.Main.Knockback.Distance;
            hitEvent.KnockbackStrength = (int)hit.Main.Knockback.Strength;
            hitEvent.KnockStopFrame = hit.Main.Knockback.KnockStopFrame;
            hitEvent.FinalDamage = hit.Damage.FinalDamage;
            hitEvent.DpDamage = hit.Damage.DpDamage;
            hitEvent.ServerFrame = BoltNetwork.ServerFrame + 1;
            hitEvent.Position = hit.Main.Position;
            hitEvent.Killed = hit.Killed;
            hitEvent.IsCritical = hit.Damage.IsCritical;
            hitEvent.HitstopReductionIdx = hit.Main.HitstopReductionIdx;
            hitEvent.SuperArmor = hit.Main.SuperArmor;
            hitEvent.IsDirect = hit.Main.IsDirect;
            return hitEvent;
        }

        private HealEvent CreateHealEvent(HealResultInfo heal)
        {
            var healEvent = HealEvent.Create(GlobalTargets.Everyone, ReliabilityModes.Unreliable);
            healEvent.Target = heal.Target.EntityId;
            healEvent.RealAmount = heal.RealAmount;
            return healEvent;
        }

        private StunEvent CreateStunEvent(GameCommandStun stun)
        {
            // TODO : 모두에게 보낼 필요는 없긴함. Controller 와 Owner 만 보내도된다.
            var stunEvent = StunEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            stunEvent.Target = stun.Target?.EntityId ?? -1;
            stunEvent.Frame = stun.Frame;
            stunEvent.ServerFrame = BoltNetwork.ServerFrame + 1;
            return stunEvent;
        }

        private ParryingSuccessEvent CreateParryingSuccessEvent(ParryingInfo parryingInfo)
        {
            // TODO : 모두에게 보낼 필요는 없긴함. Controller 와 Owner 만 보내도된다.
            var parryingEvent = ParryingSuccessEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            parryingEvent.Attacker = parryingInfo.Attacker?.EntityId ?? -1;
            parryingEvent.Victim = parryingInfo.Victim?.EntityId ?? -1;
            parryingEvent.HitFxId = HitFxTable.Instance.GetId(parryingInfo.HitFx);
            parryingEvent.ServerFrame = BoltNetwork.ServerFrame + 1;
            parryingEvent.Direction = parryingInfo.Direction;
            return parryingEvent;
        }

        private ForceActionEvent CreateForceActionEvent(GameCommandForceAction forceAction)
        {
            var character = forceAction.Entity as CharacterBehaviour;
            var actionIdx = -1;
            if (character != null)
                actionIdx = character.CharacterData.PossibleActions.IndexOf(forceAction.ActionData);

            // TODO : 모두에게 보낼 필요는 없긴함. Controller 와 Owner 만 보내도된다.
            var forceActionEvent = ForceActionEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            forceActionEvent.EntityIdx = forceAction.Entity?.EntityId ?? -1;
            forceActionEvent.ActionIdx = actionIdx;
            forceActionEvent.ServerFrame = BoltNetwork.ServerFrame + 1;
            return forceActionEvent;
        }

        private ReviveEvent CreateReviveEvent(GameCommandRevive revive)
        {
            // TODO : 모두에게 보낼 필요는 없긴함. Controller 와 Owner 만 보내도된다.
            var reviveEvent = ReviveEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            reviveEvent.EntityId = revive.Target?.EntityId ?? -1;
            reviveEvent.ServerFrame = BoltNetwork.ServerFrame + 1;
            return reviveEvent;
        }

        private AddStackEvent CreateAddStackEvent(GameCommandAddStack addStack)
        {
            // 서버에서만 처리하는 이벤트
            var addStackEvent = AddStackEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);
            addStackEvent.EntityId = addStack.Target?.EntityId ?? -1;
            addStackEvent.Value = addStack.Count;
            addStackEvent.ServerFrame = BoltNetwork.ServerFrame + 1;
            return addStackEvent;
        }
    }
}