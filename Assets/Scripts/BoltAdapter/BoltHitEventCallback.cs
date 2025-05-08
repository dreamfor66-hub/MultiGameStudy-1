using System.Collections.Generic;
using FMLib.Extensions;
using FMLib.Structs;
using Photon.Bolt;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Event;
using Rogue.Ingame.Input;
using Rogue.Ingame.Vfx;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour]
    public class BoltHitEventCallback : GlobalEventListener
    {
        private readonly List<ICharacterController> controllers = new List<ICharacterController>();

        public override void OnEvent(HitEvent evnt)
        {
            var hitInfo = CreateHitInfo(evnt);
            var attackerIsMe = false;
            var victimIsMe = false;

            var victim = controllers.Find(x => x.CharacterId == evnt.Victim);
            if (victim != null)
            {
                victim.Hurt(hitInfo, evnt.ServerFrame);
                victim.Character.CharacterMaterial.OnHurt(evnt);
                var hitstop = HitstopInfo.CreateVictim(evnt.HitFxId);
                victim.Hitstop(hitstop, evnt.ServerFrame);
                victimIsMe = victim.IsMe;
            }

            var attacker = controllers.Find(x => x.CharacterId == evnt.Attacker);
            if (attacker != null)
            {
                var hitstop = HitstopInfo.CreateAttacker(evnt.HitFxId, evnt.HitstopReductionIdx);
                attacker.Hitstop(hitstop, evnt.ServerFrame);
            }

            var attackerBullet = EntityTable.FindById(evnt.Attacker) as BulletBehaviour;
            if (attackerBullet != null)
                attackerBullet.Hitstop();

            var attackerRoot = controllers.Find(x => x.CharacterId == evnt.AttackerRoot);
            if (attackerRoot != null)
            {
                attackerIsMe = attackerRoot.IsMe;
            }

            if (victim != null && evnt.FinalDamage != 0) // Bullet 타격시 대미지 표기 X
            {
                if (attackerIsMe || victimIsMe)
                    EventDispatcher.Send(new EventDamage(evnt.FinalDamage, evnt.IsCritical, evnt.Position));
            }

            Hit(hitInfo, attackerIsMe, victimIsMe);
            BoltBulletManager.Instance.Hit(hitInfo);
        }

        public override void OnEvent(StunEvent evnt)
        {
            var target = controllers.Find(x => x.CharacterId == evnt.Target);
            if (target != null)
            {
                target.Stun(evnt.Frame, evnt.ServerFrame);
            }
        }

        public override void OnEvent(ParryingSuccessEvent evnt)
        {
            var attacker = controllers.Find(x => x.CharacterId == evnt.Attacker);
            if (attacker != null)
            {
                var hitstop = HitstopInfo.CreateParrying(evnt.HitFxId);
                attacker.Hitstop(hitstop, evnt.ServerFrame);
            }

            var victim = controllers.Find(x => x.CharacterId == evnt.Victim);
            if (victim != null && evnt.Direction != Vector3.zero)
            {
                victim.SetDirection(new VectorXZ(evnt.Direction), evnt.ServerFrame);
            }
        }

        public override void OnEvent(ForceActionEvent evnt)
        {
            var target = controllers.Find(x => x.CharacterId == evnt.EntityIdx);
            if (target != null)
                target.ForceAction(evnt.ActionIdx, evnt.ServerFrame);
        }

        public override void OnEvent(ReviveEvent evnt)
        {
            var target = controllers.Find(x => x.CharacterId == evnt.EntityId);
            if (target != null)
                target.Revive(evnt.ServerFrame);
        }

        public override void OnEvent(AddStackEvent evnt)
        {
            var target = controllers.Find(x => x.CharacterId == evnt.EntityId);
            if (target != null)
                target.AddStack(evnt.Value, evnt.ServerFrame);
        }

        public override void EntityAttached(BoltEntity entity)
        {
            var controller = GetCharacterController(entity);
            if (controller != null)
            {
                controllers.Add(controller);
            }
        }

        public override void EntityDetached(BoltEntity entity)
        {
            var controller = GetCharacterController(entity);
            if (controller != null)
            {
                controllers.Remove(controller);
            }
        }


        private ICharacterController GetCharacterController(BoltEntity entity)
        {
            var playerController = entity.gameObject.GetComponent<BoltPlayerController>();
            if (playerController != null)
                return playerController;

            var npcController = entity.gameObject.GetComponent<BoltNpcController>();
            if (npcController != null)
                return npcController;
            return null;
        }

        private void Hit(HitResultInfo hitInfo, bool attackerIsMe, bool victimIsMe)
        {
            var hiFx = hitInfo.Main.HitFx;
            var position = hitInfo.Main.Position;
            if (hiFx == null)
            {
                return;
            }

            if (hiFx.HitClip != null)
                AudioSource.PlayClipAtPoint(hiFx.HitClip, position, 0.5f);

            if (hiFx.HitVfx != null)
                VfxSpawner.Spawn(hiFx.HitVfx, position, 1f);

            EventDispatcher.Send(new EventAttackHit(hitInfo, attackerIsMe, victimIsMe));
        }

        private HitResultInfo CreateHitInfo(HitEvent evnt)
        {
            HitResultInfo info;
            info.Main.Attacker = EntityTable.FindById(evnt.Attacker);
            info.Main.AttackerRoot = EntityTable.FindById(evnt.AttackerRoot);
            info.Main.Victim = EntityTable.FindById(evnt.Victim);
            info.Main.HitFx = HitFxTable.Instance.GetById(evnt.HitFxId);
            info.Main.Knockback = CreateKnockback(evnt);
            info.Main.Position = evnt.Position;
            info.Main.HitstopReductionIdx = evnt.HitstopReductionIdx;
            info.Main.SuperArmor = evnt.SuperArmor;
            info.Main.IsDirect = evnt.IsDirect;

            info.Damage.FinalDamage = evnt.FinalDamage;
            info.Damage.DpDamage = evnt.DpDamage;
            info.Damage.IsCritical = evnt.IsCritical;

            info.Killed = evnt.Killed;
            return info;
        }

        public KnockbackInfo CreateKnockback(HitEvent evnt)
        {
            KnockbackInfo info;
            info.Direction = DirectionEncoder.FromAngle(evnt.KnockbackDirection);
            info.Distance = evnt.KnockbackDistance;
            info.Strength = (KnockbackStrength)evnt.KnockbackStrength;
            info.KnockStopFrame = evnt.KnockStopFrame;
            return info;
        }
    }
}