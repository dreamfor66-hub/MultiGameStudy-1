using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Photon.Bolt;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Event;
using Rogue.Ingame.Goods;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Debug = UnityEngine.Debug;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour]
    public class BoltRewardManager : GlobalEventListener
    {
        public static BoltRewardManager Instance
        {
            get;
            private set;
        }

        private void Awake()
        {
            Instance = this;
        }

        IEnumerator Start()
        {
            while (RewardModel.Instance == null)
                yield return null;
            Init();
        }

        private bool isInit = false;

        private void Init()
        {
            if (isInit)
                return;
            isInit = true;
            RewardModel.Instance.BuffChangeModel.OnAddBuff += AddBuffClient;
            RewardModel.Instance.BuffChangeModel.OnRemoveBuff += RemoveBuffClient;
            GoodsModel.Instance.ItemBuffModel.OnAddBuff += AddBuffClient;
        }

        private void Destroy()
        {
            Instance = null;
            if (isInit)
            {
                RewardModel.Instance.BuffChangeModel.OnAddBuff -= AddBuffClient;
                RewardModel.Instance.BuffChangeModel.OnRemoveBuff -= RemoveBuffClient;
                GoodsModel.Instance.ItemBuffModel.OnAddBuff -= AddBuffClient;
            }
        }

        private readonly List<BoltConnection> waitConnections = new List<BoltConnection>();

        public void CoinReward(int amount)
        {
            var target = BoltServerCallbacks.IsDedicated ? GlobalTargets.AllClients : GlobalTargets.Everyone;
            var evt = CoinRewardEvent.Create(target, ReliabilityModes.ReliableOrdered);
            evt.Amount = amount;
            evt.Send();
        }

        // 서버
        public async Task RewardServerAsync(int level)
        {
            waitConnections.Clear();
            waitConnections.AddRange(BoltPlayerObjectRegistry.AllPlayers.Select(x => x.Conneciton));

            var target = BoltServerCallbacks.IsDedicated ? GlobalTargets.AllClients : GlobalTargets.Everyone;


            var evt = RewardStartEvent.Create(target, ReliabilityModes.ReliableOrdered);
            evt.Level = level;
            evt.Send();

            var timer = new Stopwatch();
            timer.Start();
            while (true)
            {
                if (waitConnections.Count == 0)
                    break;
                if (timer.Elapsed > TimeSpan.FromSeconds(30f))
                    break;
                await Task.Delay(500);
            }
        }

        // 서버
        public override void OnEvent(RewardBuffEvent evnt)
        {
            var conn = evnt.RaisedBy;
            var player = BoltPlayerObjectRegistry.GetPlayer(evnt.RaisedBy);
            var character = player.Character.GetComponent<CharacterBehaviour>();

            var buff = BuffTable.Instance.GetById(evnt.BuffId);
            if (evnt.IsAdd)
                character.BuffAccepter.AddBuff(buff, character);
            else
                character.BuffAccepter.RemoveBuff(buff);
        }

        //서버
        public override void OnEvent(RewardEndEvent evnt)
        {
            var conn = evnt.RaisedBy;
            waitConnections.Remove(conn);
        }

        //클라
        public override void OnEvent(CoinRewardEvent evnt)
        {
            if (!OwnerCharacterHolder.OwnerCharacterExistAndAlive)
                return;
            var amount = evnt.Amount;
            GoodsModel.Instance.MoneyModel.Gain(amount);
            var player = OwnerCharacterHolder.OwnerCharacter;
            if (player != null)
            {
                EventDispatcher.Send(new EventCoinGet(amount, player));
            }
        }

        public override void OnEvent(ResetDungeonEvent evnt)
        {
            RewardModel.Instance.Reset();
            GoodsModel.Instance.Reset();
        }

        // 클라
        public override async void OnEvent(RewardStartEvent evnt)
        {
            try
            {
                Init();
                if (OwnerCharacterHolder.OwnerCharacterExistAndAlive)
                    await RewardManager.Instance.Reward(evnt.Level);
                EndRewardClient();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        // 클라
        public void AddBuffClient(BuffData buff)
        {
            var evt = RewardBuffEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);
            evt.IsAdd = true;
            evt.BuffId = BuffTable.Instance.GetId(buff);
            evt.Send();
        }

        // 클라
        public void RemoveBuffClient(BuffData buff)
        {
            var evt = RewardBuffEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);
            evt.IsAdd = false;
            evt.BuffId = BuffTable.Instance.GetId(buff);
            evt.Send();
        }

        public void EndRewardClient()
        {
            var evt = RewardEndEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);
            evt.Send();
        }
    }
}