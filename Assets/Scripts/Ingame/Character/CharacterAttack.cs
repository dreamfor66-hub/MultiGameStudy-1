using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Factory;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Core;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Character
{
    public class CharacterAttack : IAttacker
    {
        public ActionData ActionData => actionData;
        public float Frame => curFrame;

        private readonly CharacterData characterData;
        private readonly IEntity entity;
        private readonly HitColliderAnchor[] anchors;

        public CharacterAttack(Transform tm, CharacterData characterData, IEntity entity)
        {
            this.characterData = characterData;
            this.entity = entity;
            anchors = tm.GetComponentsInChildren<HitColliderAnchor>();
        }

        private readonly HitboxGroupMemory groupMemory = new HitboxGroupMemory();

        private ActionData actionData;
        private float curFrame;
        private float prevFrame;

        public void UpdateAttack(CharacterStateUpdateInfo update)
        {
            if (update.Prev.Id != update.Cur.Id)
                ClearState();

            actionData = update.Cur.StateType == CharacterStateType.Action ? update.Cur.ActionData : null;
            curFrame = update.Cur.Frame;
            prevFrame = update.Cur.Frame - update.DeltaFrame;
        }

        public void MemoryPosition()
        {
            foreach (var anchor in anchors)
                anchor.MemoryPosition();
        }

        public HitColliderAnchor GetAnchor(int id)
        {
            var anchor = anchors.FirstOrDefault(x => x.Id == id);
            if (anchor == null)
                throw new Exception($"can't find anchor : {id}");
            return anchor;
        }

        private void ClearState()
        {
            groupMemory.Clear();
        }

        public void Cast()
        {
            if (actionData == null)
                return;

            foreach (var hitbox in actionData.AttackHitboxData)
            {
                CastHitbox(hitbox, actionData);
            }
        }

        private List<Hurtbox> hurts = new List<Hurtbox>();

        private void CastHitbox(AttackHitboxData hitbox, ActionData action)
        {
            if (curFrame < hitbox.StartFrame)
                return;
            if (prevFrame > hitbox.EndFrame)
                return;

            var prevRatio = prevFrame < hitbox.StartFrame
                ? (hitbox.StartFrame - prevFrame) / (curFrame - prevFrame)
                : 1f;
            var curRatio = curFrame > hitbox.EndFrame
                ? (hitbox.EndFrame - prevFrame) / (curFrame - prevFrame)
                : 1f;

            var anchor = GetAnchor(hitbox.ColliderId);

            hurts.Clear();
            anchor.Cast(ref hurts, prevRatio, curRatio);

            foreach (var hurtbox in hurts.Distinct())
            {
                if (hurtbox.Entity.GameObject == entity.GameObject)
                    continue;
                if (!AttackTeamHelper.Attackable(entity.Team, hurtbox))
                    continue;
                if (groupMemory.Contains(hitbox.GroupId, hurtbox.Entity))
                    continue;

                if (hurtbox.NowParrying)
                {
                    var info = HitInfoFactory.CreateParrying(entity, hurtbox.Entity, hitbox.Info);
                    GameCommandParryingSuccess.Send(info);
                    BuffTriggerDispatcher.ParryingHit(entity, hurtbox.Entity);
                }
                else
                {
                    var hitPos = hurtbox.Position + Random.insideUnitSphere * .5f;
                    var prevHitCount = groupMemory.GetCount(hitbox.GroupId);
                    var hitstopIdx = HitstopReduce.GetIdx(prevHitCount);
                    var hitInfo = HitInfoFactory.Create(entity, entity, hurtbox.Entity, hitbox.Info, hitPos, action.AttackType, true, hitstopIdx, hurtbox.NowSuperArmor, 0, 0);
                    GameCommandHitAttack.Send(hitInfo);

                    if (hitbox.BuffData != null)
                        GameCommandAddBuff.Send(entity, hurtbox.Entity, hitbox.BuffData);
                }

                groupMemory.Add(hitbox.GroupId, hurtbox.Entity);
            }
        }

    }
}