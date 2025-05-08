using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Factory;
using Rogue.Ingame.Character;
using Rogue.Ingame.Core;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Bullet
{
    public class BulletAttack : IAttacker
    {
        private readonly IEntity entity;
        private readonly BulletAttackData attackData;
        private readonly HitColliderAnchor[] anchors;
        private readonly HashSet<Hurtbox> hurted = new HashSet<Hurtbox>();

        private BulletServerInfo serverInfo;
        private int curFrame;
        private readonly Queue<Data.BulletEvent> evtQueue = new Queue<Data.BulletEvent>();
        private readonly HitboxGroupMemory groupMemory = new HitboxGroupMemory();

        public BulletAttack(Transform tm, IEntity entity, BulletAttackData attackData)
        {
            this.entity = entity;
            this.attackData = attackData;
            anchors = tm.GetComponentsInChildren<HitColliderAnchor>();
        }

        public void Reset(BulletServerInfo info)
        {
            this.serverInfo = info;
            MemoryPosition();
        }

        public void OnEvent(Data.BulletEvent evt)
        {
            evtQueue.Enqueue(evt);
            var attacks = attackData.Attacks.Where(x =>
                x.Condition.Type == BulletAttackConditionType.Event && x.Condition.Event == evt);

            foreach (var attack in attacks)
            {
                Cast(attack, false);
            }
        }

        public void UpdateFrame(int frame)
        {
            curFrame = frame;
            MemoryPosition();
        }

        public void OnDespawn()
        {
            hurted.Clear();
            groupMemory.Clear();
        }

        public void MemoryPosition()
        {
            foreach (var anchor in anchors)
                anchor.MemoryPosition();
        }

        private List<Hurtbox> hurts = new List<Hurtbox>();
        public void Cast()
        {
            foreach (var attack in attackData.Attacks)
            {
                if (attack.Condition.Type == BulletAttackConditionType.Always)
                {
                    var connect = curFrame > 0;
                    Cast(attack, connect);
                }
                else if (attack.Condition.Type == BulletAttackConditionType.Frame)
                {
                    if (attack.Condition.MinFrame <= curFrame && curFrame <= attack.Condition.MaxFrame)
                    {
                        var connect = attack.Condition.MinFrame < curFrame;
                        Cast(attack, connect);
                    }
                }
            }
        }

        private HitColliderAnchor GetAnchor(int id)
        {
            return anchors.FirstOrDefault(x => x.Id == id);
        }

        private void Cast(BulletSingleAttackData attack, bool connectPrev)
        {
            var anchor = GetAnchor(attack.AnchorId);
            if (anchor == null)
            {
                Debug.LogError($"can't find anchor : {attack.AnchorId}", entity.GameObject);
                return;
            }

            hurts.Clear();
            anchor.Cast(ref hurts, connectPrev ? 0f : 1f, 1f);

            foreach (var hurtbox in hurts.Distinct())
            {
                if (hurtbox.Entity.GameObject == entity.GameObject)
                    continue;
                if (attack.TeamFilter == BulletAttackTeamFilter.Attackable)
                    if (!AttackTeamHelper.Attackable(entity.Team, hurtbox))
                        continue;
                if (attack.TeamFilter == BulletAttackTeamFilter.SameTeam)
                    if (!AttackTeamHelper.IsTeam(entity.Team, hurtbox.Team))
                        continue;
                if (groupMemory.Contains(attack.GroupId, hurtbox.Entity))
                    continue;
                if (!attack.BuffOnly)
                {
                    var hitPos = hurtbox.Position + Random.insideUnitSphere * .5f;
                    var hitInfo = HitInfoFactory.Create(serverInfo.RootSource, entity, hurtbox.Entity, attack.HitboxInfo,
                        hitPos, serverInfo.ActionType, attack.IsDirectHit, 0, hurtbox.NowSuperArmor, serverInfo.AdditionalCriticalChance, serverInfo.AdditionalCriticalDamagePercent);
                    GameCommandHitAttack.Send(hitInfo);
                }

                if (attack.BuffData != null)
                {
                    GameCommandAddBuff.Send(serverInfo.RootSource, hurtbox.Entity, attack.BuffData);
                }

                groupMemory.Add(attack.GroupId, hurtbox.Entity);
                if (attack.HitOnce)
                    break;
            }
        }
    }
}