using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff
{
    public class BuffInstance
    {
        public int Id { get; }
        public int RemainFrame => endFrame - elapsedFrame;
        public int Duration { get; private set; }
        public int StackCount { get; private set; }
        public bool IsEnd { get; private set; }
        public BuffData Data => data;

        private readonly BuffData data;
        private readonly BuffCondition buffCondition;
        private readonly BuffValues buffValues;
        //private readonly BuffVfxLegacy buffVfx;
        private readonly IEntity me;
        private readonly IEntity rootSource;

        private readonly List<BuffUnit> units;

        private int elapsedFrame;
        private int endFrame;

        public BuffInstance(int id, BuffData data, BuffCondition buffCondition, BuffValues buffValues, IEntity me, IEntity rootSource)
        {
            Id = id;
            this.data = data;
            this.buffCondition = buffCondition;
            this.buffValues = buffValues;
            this.me = me;
            this.rootSource = rootSource;

            //buffVfx = new BuffVfxLegacy(me.GameObject.transform, data.VfxData);
            units = data.Buffs.Select(CreateUnit).ToList();
            elapsedFrame = 0;
            StackCount = 1;
            IsEnd = false;

            var timeCondition =
                data.ReleaseCondition.Find(x => x.ReleaseConditionType == BuffReleaseConditionType.Time);
            endFrame = timeCondition.TimeFrame;
            Duration = endFrame;
        }

        private BuffUnit CreateUnit(BuffUnitData unitData)
        {
            return new BuffUnit(unitData, buffCondition, buffValues, this, me, rootSource);
        }

        public void OnStart()
        {
            foreach (var unit in units)
            {
                unit.OnStart();
            }
            //buffVfx.OnStart();
        }

        public void OnUpdate()
        {
            foreach (var unit in units)
            {
                unit.OnUpdate();
            }
            //buffVfx.OnUpdate(elapsedFrame);
            elapsedFrame++;
            CheckRelease();
        }

        public void OnEnd()
        {
            foreach (var unit in units)
            {
                unit.OnEnd();
            }
            //buffVfx.OnEnd();
        }

        public void SetEnd()
        {
            IsEnd = true;
        }

        public void CheckRelease()
        {
            if (IsEnd)
                return;
            IsEnd = data.ReleaseCondition.Any(CheckRelease);
            if (me is CharacterBehaviour character)
                IsEnd |= character.IsDead && !BuffTagHelper.SustainIfOwnerDead(data.Tags);
        }

        private bool CheckRelease(BuffReleaseCondition releaseCondition)
        {
            switch (releaseCondition.ReleaseConditionType)
            {
                case BuffReleaseConditionType.Time:
                    return elapsedFrame >= endFrame;
                case BuffReleaseConditionType.AttackDo:
                    {
                        if (me is CharacterBehaviour character)
                        {
                            var stateInfo = character.TotalInfo.StateInfo;
                            if (stateInfo.StateType == CharacterStateType.Action)
                            {
                                var actionData = stateInfo.ActionData;
                                return actionData.IsAttack;
                            }
                        }

                        return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RefreshTime(int timeFrame)
        {
            var newFrame = elapsedFrame + timeFrame;
            endFrame = Mathf.Max(newFrame, endFrame);
        }

        public void AddTime(int timeFrame, bool isDurationChanged)
        {
            if (isDurationChanged)
                Duration = Mathf.Max(1, Duration + timeFrame);
            endFrame += timeFrame;
        }

        public void Stack()
        {
            ChangeStack(StackCount + 1);
        }

        public void ChangeStack(int count)
        {
            if (count >= data.OverlapMethod.MaxStackCount)
            {
                count = data.OverlapMethod.MaxStackCount;
            }
            if (count == StackCount)
            {
                return;
            }
            StackCount = count;
            foreach (var unit in units)
            {
                unit.OnStackCountChanged(StackCount);
            }
            BuffTriggerDispatcher.BuffStackChanged(me, this);

            if (count <= 0)
                IsEnd = true;
        }

        public void OverrideHit(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            foreach (var unit in units)
            {
                unit.OverrideHit(hitInfo, ref buffInfo);
            }
        }

        public void OverrideHurt(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            foreach (var unit in units)
            {
                unit.OverrideHurt(hitInfo, ref buffInfo);
            }
        }
    }
}