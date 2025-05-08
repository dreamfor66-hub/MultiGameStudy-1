using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Event;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff
{
    public struct BuffInfo
    {
        public int EntityId;
        public int BuffInstanceId;
        public BuffData BuffData;
        public int DurationFrame;
        public int RemainFrame;
        public int StackCount;
    }

    public struct BuffEndInfo
    {
        public int EntityId;
        public int BuffInstanceId;
        public BuffData BuffData;
    }

    public class BuffAccepter
    {
        private readonly BuffCondition buffCondition;
        private readonly BuffValues buffValues;
        private readonly IEntity me;

        public Action<BuffInstance> OnStartBuffLegacy;
        public Action<BuffInstance> OnEndBuffLegacy;

        public Action<BuffInfo> OnStartBuff;
        public Action<BuffInfo> OnChangeBuff;
        public Action<BuffEndInfo> OnEndBuff;

        private readonly List<BuffInstance> buffs = new List<BuffInstance>();
        public IReadOnlyList<BuffInstance> GetBuffs() => buffs;
        private static int idGen = 1; // 한 캐릭터 안에서 Unique
        private static int GenerateId()
        {
            return idGen++;
        }

        public BuffAccepter(BuffCondition buffCondition, BuffValues buffValues, IEntity me)
        {
            this.buffCondition = buffCondition;
            this.buffValues = buffValues;
            this.me = me;

            buffCondition.LinkBuffAccepter(buffs);
        }

        public void UpdateFrame()
        {
            foreach (var buff in buffs)
            {
                buff.OnUpdate();
            }

            foreach (var buff in buffs)
            {
                if (buff.IsEnd)
                {
                    EndBuff(buff);
                }
            }

            buffs.RemoveAll(x => x.IsEnd);
        }

        public void Clear()
        {
            foreach (var buff in buffs)
            {
                EndBuff(buff);
            }
            buffs.Clear();
        }

        public void AddBuff(BuffData buffData, IEntity rootSource)
        {
            if (buffData == null)
                throw new Exception("null buff data");

            var prev = buffs.Find(x => IsSameGroup(x, buffData));
            if (prev == null)
            {
                AddBuffCore(buffData, rootSource);
            }
            else
            {
                switch (buffData.OverlapMethod.OverlapType)
                {
                    case BuffOverlapType.Individual:
                        AddBuffCore(buffData, rootSource);
                        break;
                    case BuffOverlapType.RefreshTimeOnly:
                        {
                            var timeFrame = GetBuffTimeFrame(buffData);
                            prev.RefreshTime(timeFrame);
                            OnChangeBuff?.Invoke(CreateInfo(prev));
                            break;
                        }
                    case BuffOverlapType.StackAndRefreshTime:
                        {
                            var timeFrame = GetBuffTimeFrame(buffData);
                            prev.RefreshTime(timeFrame);
                            prev.Stack();
                            OnChangeBuff?.Invoke(CreateInfo(prev));
                            break;
                        }
                    case BuffOverlapType.IgnoreNew:
                        break;
                    case BuffOverlapType.ReplaceWithNew:
                        RemoveBuff(prev);
                        AddBuffCore(buffData, rootSource);
                        break;
                    case BuffOverlapType.SelectHigherLevelAndRefreshTime:
                        {
                            if (prev.Data.OverlapMethod.Level >= buffData.OverlapMethod.Level)
                            {
                                var timeFrame = GetBuffTimeFrame(buffData);
                                prev.RefreshTime(timeFrame);
                                OnChangeBuff?.Invoke(CreateInfo(prev));
                            }
                            else
                            {
                                var prevTime = prev.RemainFrame;
                                var newTime = GetBuffTimeFrame(buffData);
                                RemoveBuff(prev);
                                var newBuff = AddBuffCore(buffData, rootSource);
                                if (prevTime > newTime)
                                {
                                    newBuff.RefreshTime(prevTime);
                                    OnChangeBuff?.Invoke(CreateInfo(newBuff));
                                }
                            }

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void ChangeBuffTime(BuffIdentifer buffId, int value, bool isDurationChanged)
        {
            var finds = FindBuff(buffId);
            foreach (var buff in finds)
            {
                buff.AddTime(value, isDurationChanged);
                OnChangeBuff?.Invoke(CreateInfo(buff));
            }
        }

        public void AddBuffStack(BuffIdentifer buffId, int value)
        {
            var finds = FindBuff(buffId);
            foreach (var buff in finds)
                buff.ChangeStack(buff.StackCount + value);
        }

        public void RemoveBuff(BuffData buffData)
        {
            var find = buffs.Find(x => x.Data == buffData);
            if (find != null)
            {
                RemoveBuff(find);
            }
        }

        public void ReleaseBuff(BuffIdentifer buffId)
        {
            var finds = FindBuff(buffId);
            foreach (var buff in finds)
                buff.SetEnd();
        }

        private void RemoveBuff(BuffInstance buff)
        {
            EndBuff(buff);
            buffs.Remove(buff);
        }

        private void EndBuff(BuffInstance buff)
        {
            buff.OnEnd();
            OnEndBuff?.Invoke(CreateEndInfo(buff));
            OnEndBuffLegacy?.Invoke(buff);
            EventDispatcher.Send(new EventChangeBuff(me, buff.Data, false));
        }

        public bool HasBuff(BuffTag tag)
        {
            return buffs.Any(x => x.Data.Tags.Any(y => y == tag));
        }

        private BuffInstance AddBuffCore(BuffData buffData, IEntity rootSource)
        {
            var buff = new BuffInstance(GenerateId(), buffData, buffCondition, buffValues, me, rootSource);
            buffs.Add(buff);
            buff.OnStart();
            OnStartBuff?.Invoke(CreateInfo(buff));
            OnStartBuffLegacy?.Invoke(buff);
            EventDispatcher.Send(new EventChangeBuff(me, buffData, true));
            return buff;
        }


        private BuffInfo CreateInfo(BuffInstance instance)
        {
            BuffInfo info;
            info.EntityId = me.EntityId;
            info.BuffInstanceId = instance.Id;
            info.BuffData = instance.Data;
            info.DurationFrame = instance.Duration;
            info.RemainFrame = instance.RemainFrame;
            info.StackCount = instance.StackCount;
            return info;
        }

        private BuffEndInfo CreateEndInfo(BuffInstance instance)
        {
            BuffEndInfo endInfo;
            endInfo.EntityId = me.EntityId;
            endInfo.BuffInstanceId = instance.Id;
            endInfo.BuffData = instance.Data;
            return endInfo;
        }

        private int GetBuffTimeFrame(BuffData data)
        {
            return data.ReleaseCondition.Find(x => x.ReleaseConditionType == BuffReleaseConditionType.Time).TimeFrame;
        }

        private bool HasTimeCondition(BuffData data)
        {
            return data.ReleaseCondition.Any(x => x.ReleaseConditionType == BuffReleaseConditionType.Time);
        }

        private bool IsSameGroup(BuffInstance prevBuff, BuffData newBuffData)
        {
            if (newBuffData.OverlapMethod.ShowGroup)
                return prevBuff.Data.Tags.Any(x => x == newBuffData.OverlapMethod.Tag);
            else
                return prevBuff.Data == newBuffData;
        }

        public bool IsAbnormalStatus()
        {
            return buffs.Any(x => x.Data.Tags.Any(t => t == BuffTag.AbnormalStatus));
        }

        public void OverrideHit(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            foreach (var buff in buffs)
            {
                buff.OverrideHit(hitInfo, ref buffInfo);
            }
        }

        public void OverrideHurt(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            foreach (var buff in buffs)
            {
                buff.OverrideHurt(hitInfo, ref buffInfo);
            }
        }

        private IEnumerable<BuffInstance> FindBuff(BuffIdentifer buffId)
        {
            if (buffId.Tag == BuffTag.Self)
                return buffs.Where(x => x.Id == buffId.SelfId);
            else
                return buffs.Where(x => x.Data.Tags.Any(t => t == buffId.Tag));
        }
    }
}