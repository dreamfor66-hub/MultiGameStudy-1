using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour]
    public class BoltBuffSync : GlobalEventListener
    {
        BuffStartEvent evnt = null;
        private bool isInit = false;

        internal struct SameBuff
        {
            internal int EntityId;
            internal int DataId;

        }
        Dictionary<SameBuff, int> missingBuffDic = new Dictionary<SameBuff, int>();
        private void Awake()
        {
            if (BoltNetwork.IsServer)
            {
                EntityTable.OnAdd += OnEntityAdd;
                EntityTable.OnRemove += OnEntityRemove;
                isInit = true;
            }
        }

        private void OnDestroy()
        {
            if (isInit)
            {
                EntityTable.OnAdd -= OnEntityAdd;
                EntityTable.OnRemove -= OnEntityRemove;
            }
        }

        private void RegisterBuffCallbacks(CharacterBehaviour character)
        {
            character.BuffAccepter.OnStartBuff += OnStartBuff;
            character.BuffAccepter.OnChangeBuff += OnChangeBuff;
            character.BuffAccepter.OnEndBuff += OnEndBuff;
        }

        private void UnRegisterBuffCallbacks(CharacterBehaviour character)
        {
            character.BuffAccepter.OnStartBuff -= OnStartBuff;
            character.BuffAccepter.OnChangeBuff -= OnChangeBuff;
            character.BuffAccepter.OnEndBuff -= OnEndBuff;
        }

        private void OnEntityAdd(IEntity entity)
        {
            if (entity is CharacterBehaviour character)
                RegisterBuffCallbacks(character);
        }

        private void OnEntityRemove(IEntity entity)
        {
            if (entity is CharacterBehaviour character)
                UnRegisterBuffCallbacks(character);
        }

        IEnumerator WaitTillSpawn(BuffStartEvent evnt, float Delay)
        {
            yield return new WaitForSeconds(Delay);
            OnEvent(evnt);
        }

        public override void OnEvent(BuffStartEvent evnt)
        {
            var entity = EntityTable.FindById(evnt.EntityId);
            if (entity is CharacterBehaviour character)
            {
                character.BuffSync.OnStartBuff?.Invoke(CreateBuffInfo(evnt));
            }
            else
            {
                SameBuff curBuff = new SameBuff
                {
                    EntityId = evnt.EntityId,
                    DataId = evnt.DataId
                };

                if (!missingBuffDic.TryGetValue(curBuff, out var count) || count < 6)
                {
                    var resendEvnt = BuffStartEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);
                    resendEvnt.EntityId = evnt.EntityId;
                    resendEvnt.InstanceId = evnt.InstanceId;
                    resendEvnt.DataId = evnt.DataId;
                    resendEvnt.DurationFrame = evnt.DurationFrame;
                    resendEvnt.RemainFrame = evnt.RemainFrame;
                    resendEvnt.StackCount = evnt.StackCount;
                    StartCoroutine(WaitTillSpawn(resendEvnt, 0.5f));
                    if (missingBuffDic.ContainsKey(curBuff))
                    {
                        missingBuffDic[curBuff]++;
                    }
                    else
                    {
                        missingBuffDic.Add(curBuff, 1);
                    }
                }
                else
                {
                    Debug.LogError($"3회 다 사용 <color=green>{evnt.EntityId}</color>");
                }
            }
        }
        public override void OnEvent(BuffChangeEvent evnt)
        {
            var entity = EntityTable.FindById(evnt.EntityId);
            if (entity is CharacterBehaviour character)
                character.BuffSync.OnChangeBuff?.Invoke(CreateBuffInfo(evnt));
        }

        public override void OnEvent(BuffEndEvent evnt)
        {
            var entity = EntityTable.FindById(evnt.EntityId);
            if (entity is CharacterBehaviour character)
                character.BuffSync.OnEndBuff?.Invoke(CreateBuffEndInfo(evnt));
        }

        private void OnStartBuff(BuffInfo info)
        {
            if (!info.BuffData.NeedSync)
                return;
            var evnt = BuffStartEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            evnt.EntityId = info.EntityId;
            evnt.InstanceId = info.BuffInstanceId;
            evnt.DataId = BuffTable.Instance.GetId(info.BuffData);
            evnt.DurationFrame = info.DurationFrame;
            evnt.RemainFrame = info.RemainFrame;
            evnt.StackCount = info.StackCount;
            evnt.Send();
        }

        private void OnChangeBuff(BuffInfo info)
        {
            if (!info.BuffData.NeedSync)
                return;
            var evnt = BuffChangeEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            evnt.EntityId = info.EntityId;
            evnt.InstanceId = info.BuffInstanceId;
            evnt.DataId = BuffTable.Instance.GetId(info.BuffData);
            evnt.DurationFrame = info.DurationFrame;
            evnt.RemainFrame = info.RemainFrame;
            evnt.StackCount = info.StackCount;
            evnt.Send();
        }

        private void OnEndBuff(BuffEndInfo info)
        {
            if (!info.BuffData.NeedSync)
                return;
            var evnt = BuffEndEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            evnt.EntityId = info.EntityId;
            evnt.InstanceId = info.BuffInstanceId;
            evnt.DataId = BuffTable.Instance.GetId(info.BuffData);
            evnt.Send();
        }

        private BuffInfo CreateBuffInfo(BuffStartEvent evnt)
        {
            BuffInfo info;
            info.EntityId = evnt.EntityId;
            info.BuffInstanceId = evnt.InstanceId;
            info.BuffData = BuffTable.Instance.GetById(evnt.DataId);
            info.DurationFrame = evnt.DurationFrame;
            info.RemainFrame = evnt.RemainFrame;
            info.StackCount = evnt.StackCount;
            return info;
        }

        private BuffInfo CreateBuffInfo(BuffChangeEvent evnt)
        {
            BuffInfo info;
            info.EntityId = evnt.EntityId;
            info.BuffInstanceId = evnt.InstanceId;
            info.BuffData = BuffTable.Instance.GetById(evnt.DataId);
            info.DurationFrame = evnt.DurationFrame;
            info.RemainFrame = evnt.RemainFrame;
            info.StackCount = evnt.StackCount;
            return info;
        }

        private BuffEndInfo CreateBuffEndInfo(BuffEndEvent evnt)
        {
            BuffEndInfo info;
            info.EntityId = evnt.EntityId;
            info.BuffInstanceId = evnt.InstanceId;
            info.BuffData = BuffTable.Instance.GetById(evnt.DataId);
            return info;
        }
    }
}