using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Buff.Factory;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Character;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff
{
    public class BuffEffectTriggerActive
    {
        private readonly BuffEffectTriggerActiveData data;
        private readonly IEntity me;

        private readonly BuffTriggerBase trigger;
        private readonly List<IBuffActive> activeList;
        private readonly IEntity rootSource;
        private readonly BuffInstance buff;

        public BuffEffectTriggerActive(BuffEffectTriggerActiveData data, IEntity me, IEntity rootSource, BuffInstance buff)
        {
            this.data = data;
            this.me = me;
            this.rootSource = rootSource;
            this.buff = buff;

            trigger = BuffTriggerFactory.Create(me, data.TriggerData);
            activeList = data.ActiveDataList.Select(BuffActiveFactory.Create).ToList();
        }

        public void OnStart()
        {
            trigger.OnTrigger += OnTrigger;
            trigger.Start();
        }

        public void OnUpdate()
        {
            trigger.Update();
        }

        public void OnEnd()
        {
            trigger.End();
            trigger.OnTrigger -= OnTrigger;
        }


        public void OnTrigger(IEntity target)
        {
            var activeTarget = data.TargetData.ActiveTargetPivot == BuffActiveTargetType.Target ? target : me;
            var rangeType = data.TargetData.RangeType;
            var teamType = data.TargetData.TeamType;
            var entityType = data.TargetData.EntityType;
            var maxRange = data.TargetData.MaxRange;
            var maxNumber = data.TargetData.MaxNumber;
            var includePivot = data.TargetData.IncludePivot;

            if (rangeType == BuffTargetRangeType.PivotOnly)
            {
                foreach (var active in activeList)
                    active.DoActive(me, activeTarget, rootSource, buff);
            }
            else
            {
                var victims = SortByPriority(EntityTable.Entities.Where(x => CheckTeam(x.Team, teamType))
                                                                 .Where(x => CheckEntity(x, entityType))
                                                                 .Where(x => CheckRange(activeTarget, x, rangeType, maxRange)), activeTarget, rangeType);


                for (var i = 0; i < maxNumber && i < victims.Count; i++)
                {
                    if (victims[i].EntityId == activeTarget.EntityId && !includePivot)
                        victims.RemoveAt(i);
                }
                for (var i = 0; i < maxNumber && i < victims.Count; i++)
                {
                    foreach (var active in activeList)
                        active.DoActive(me, victims[i], rootSource, buff);
                }
            }
        }

        private bool CheckTeam(Team targetTeam, BuffTargetTeamType teamType)
        {
            if (teamType == BuffTargetTeamType.All)
                return true;

            var meTeam = me.Team;

            if (teamType == BuffTargetTeamType.Attackable)
            {
                return AttackTeamHelper.Attackable(meTeam, targetTeam);
            }
            else if (teamType == BuffTargetTeamType.Team)
            {
                return AttackTeamHelper.IsTeam(meTeam, targetTeam);
            }

            throw new NotImplementedException();
        }

        private bool CheckEntity(IEntity target, BuffTargetEntityType entityType)
        {
            if (target is CharacterBehaviour character)
                if (character.IsDead)
                    return false;

            if (entityType == BuffTargetEntityType.All)
                return true;

            if ((entityType & BuffTargetEntityType.Character) != 0)
                if (target.GameObject.GetComponent<CharacterBehaviour>() != null)
                    return true;

            if ((entityType & BuffTargetEntityType.Bullet) != 0)
                if (target.GameObject.GetComponent<BulletBehaviour>() != null)
                    return true;

            return false;
        }

        private bool CheckRange(IEntity src, IEntity dst, BuffTargetRangeType rangeType, float maxRange)
        {
            switch (rangeType)
            {
                //RangeType 추가될때마다 여기에 추가한다
                case BuffTargetRangeType.Nearest:
                    if (Vector3.Distance(src.GameObject.transform.position, dst.GameObject.transform.position) <= maxRange)
                        return true;
                    return false;
                case BuffTargetRangeType.PivotOnly:
                default:
                    throw new NotImplementedException();
            }
        }

        private List<IEntity> SortByPriority(IEnumerable<IEntity> entities, IEntity activeTarget, BuffTargetRangeType rangeType)
        {
            switch (rangeType)
            {
                case BuffTargetRangeType.Nearest:
                    return entities.OrderBy(x => Vector3.Distance(x.GameObject.transform.position, activeTarget.GameObject.transform.position))
                                   .ToList();
                case BuffTargetRangeType.PivotOnly:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}