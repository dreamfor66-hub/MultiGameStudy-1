using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FMLib.Random;
using Photon.Bolt;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Character;
using Rogue.Ingame.Dungeon;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using Rogue.Ingame.Stage;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Rogue.BoltAdapter
{
    public class BoltDungeonManager : EntityBehaviour<IDungeonState>
    {
        public static BoltDungeonManager Instance { get; private set; }
        private Scene currentScene;
        private StageMain stageMain;
        private DungeonModel DungeonModel => DungeonModel.Instance;

        private int clearCount = 0;
        private int failCount = 0;

        private readonly int initReviveCount = 5;

        public int ReviveCount => state.ReviveCount;

        public void Awake()
        {
            GameCommandNextStage.Listen(OnNextStage);
            Instance = this;
        }

        public void Destroy()
        {
            GameCommandNextStage.Remove(OnNextStage);
            Instance = null;
        }

        private async void OnNextStage(GameCommandNextStage cmd)
        {
            if (entity.IsOwner)
            {
                if (!DungeonModel.Instance.IsInit)
                    StartGame();
                else
                    await SelectNodeServer();
            }
        }

        public override void Attached()
        {
            state.AddCallback("StageState", OnSceneNameChanged);
            state.AddCallback("Seed", OnSeedChanged);

            if (entity.IsOwner)
            {
                state.ReviveCount = initReviveCount;
                state.StageState = CreateStageToken(-1, "Stage_Lobby");
            }
        }

        private void StartGame()
        {
            var seed = Guid.NewGuid().GetHashCode();
            state.Seed = seed;
        }

        private void OnSceneNameChanged()
        {
            var token = (StageStateToken)state.StageState;
            UnloadCurrentScene();
            StartCoroutine(LoadScene(token.SceneName));
        }

        private void OnSeedChanged()
        {
            DungeonModel.Instance.Init(state.Seed);
            if (entity.IsOwner)
            {
                var idx = DungeonModel.NodeMapModel.CurIdx;
                var sceneName = DungeonModel.StageSelectModel.CurSceneName;
                state.StageState = CreateStageToken(idx, sceneName);
            }
        }

        private IEnumerator LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            yield return null;
            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
            currentScene = scene;
            if (entity.IsOwner)
            {
                stageMain = FindObjectOfType<StageMain>();
                if (stageMain != null)
                {
                    var nodeBuff = DungeonModel.DungeonDifficultyModel.NodeTypeBuff();
                    var stageIdxValue = DungeonModel.DungeonDifficultyModel.StageIdxBuff();
                    var playerCountBuff =
                        DungeonModel.DungeonDifficultyModel.PlayerCountHpBuff(BoltPlayerObjectRegistry.PlayerCount);
                    stageMain.Init(stageIdxValue, nodeBuff, playerCountBuff, Spawn, ClearServer);
                }

                var players = FindObjectsOfType<BoltPlayerController>();
                var startPositions = FindObjectOfType<StartPositions>();
                if (startPositions != null)
                {
                    foreach (var player in players)
                    {
                        var tm = startPositions.GetTransform();
                        player.ResetPosition(tm);
                    }
                }
            }
        }

        private float gameOverTime;
        private void Update()
        {
            if (entity.IsOwner)
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    Restart();
                }
                CheckGameOver();
            }
        }

        private void CheckGameOver()
        {
            var isAllDead = false;
            var all = BoltPlayerObjectRegistry.AllPlayers.Where(x => x.Character != null)
                .Select(x => x.Character.Character);

            if (all.Count() > 0 && all.Where(x => !x.IsDead).Count() == 0)
            {
                isAllDead = true;
            }

            if (isAllDead)
            {
                gameOverTime -= Time.deltaTime;
                if (gameOverTime < 0f)
                {
                    failCount++;
                    var target = BoltServerCallbacks.IsDedicated ? GlobalTargets.AllClients : GlobalTargets.Everyone;
                    var evt = GameOverEvent.Create(target, ReliabilityModes.ReliableOrdered);
                    evt.TryCount = failCount;
                    evt.Send();
                    gameOverTime = 10f;
                    StartCoroutine(RestartAfter(2f));
                }
            }
            else
            {
                gameOverTime = 2f;
            }
        }

        private IEnumerator RestartAfter(float time)
        {
            yield return new WaitForSeconds(time);
            Restart();
        }

        private void GameClearServer()
        {
            clearCount++;
            var target = BoltServerCallbacks.IsDedicated ? GlobalTargets.AllClients : GlobalTargets.Everyone;
            var evt = GameClearEvent.Create(target, ReliabilityModes.ReliableOrdered);
            evt.TryCount = clearCount;
            evt.Send();
            StartCoroutine(RestartAfter(2f));
        }

        private void Restart()
        {
            state.StageState = CreateStageToken(-1, "Stage_Lobby");
            state.ReviveCount = initReviveCount;
            DungeonModel.Instance.End();

            foreach (var player in BoltPlayerObjectRegistry.AllPlayers)
            {
                player.Destroy();
                player.Spawn();
            }

            var monsters = EntityTable.Entities.Where(x => x.Team == Team.Monster && x is CharacterBehaviour);
            foreach (var monster in monsters)
            {
                BoltNetwork.Destroy(monster.GameObject);
            }

            var target = BoltServerCallbacks.IsDedicated ? GlobalTargets.AllClients : GlobalTargets.Everyone;
            var evt = ResetDungeonEvent.Create(target, ReliabilityModes.ReliableOrdered);
            evt.Send();
        }

        private GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return BoltNetwork.Instantiate(prefab, position, rotation).gameObject;
        }

        private void UnloadCurrentScene()
        {
            if (currentScene.isLoaded)
                SceneManager.UnloadSceneAsync(currentScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }

        public void DoRevive()
        {
            state.ReviveCount--;
        }

        public async void ClearServer()
        {
            try
            {
                if (!DungeonModel.Instance.IsInit)
                    return;

                if (DungeonModel.Instance.NodeMapModel.IsLastNode())
                    GameClearServer();
                else
                {
                    await Reward(DungeonModel.Instance.NodeMapModel.CurNode.Type);
                    await SelectNodeServer();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async Task Reward(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Normal1:
                case NodeType.Normal2:
                case NodeType.Normal3:
                case NodeType.MiddleBoss:
                    BoltRewardManager.Instance.CoinReward(Random.Range(100, 200));
                    break;
                case NodeType.NormalGold:
                    BoltRewardManager.Instance.CoinReward(Random.Range(500, 700));
                    break;
                case NodeType.Boss:
                case NodeType.Recovery:
                case NodeType.Shop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }

            await Task.Delay(1000);
            switch (nodeType)
            {
                case NodeType.Normal1:
                    await BoltRewardManager.Instance.RewardServerAsync(0);
                    break;
                case NodeType.Normal2:
                    await BoltRewardManager.Instance.RewardServerAsync(1);
                    break;
                case NodeType.Normal3:
                case NodeType.MiddleBoss:
                    await BoltRewardManager.Instance.RewardServerAsync(2);
                    break;

            }
        }



        private readonly List<BoltConnection> waitConnections = new List<BoltConnection>();
        private readonly Dictionary<Vector2Int, int> selectedCount = new Dictionary<Vector2Int, int>();
        public async Task SelectNodeServer()
        {
            waitConnections.Clear();
            selectedCount.Clear();
            waitConnections.AddRange(BoltPlayerObjectRegistry.AllPlayers.Select(x => x.Conneciton));
            var selectable = DungeonModel.Instance.NodeMapModel.SelectableNodes();
            selectable.ForEach(x => selectedCount[x] = 0);

            var target = BoltServerCallbacks.IsDedicated ? GlobalTargets.AllClients : GlobalTargets.Everyone;
            var evt = NodeSelectStartEvent.Create(target, ReliabilityModes.ReliableOrdered);
            evt.CurRow = DungeonModel.Instance.NodeMapModel.CurNode.Pos.x;
            evt.CurColumn = DungeonModel.Instance.NodeMapModel.CurNode.Pos.y;
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

            var max = selectedCount.Values.Max();
            var finalSelected = selectable.Where(x => selectedCount[x] == max).SelectN(RandomByUnity.Instance, 1).First();

            var endEvt = NodeSelectEndEvent.Create(target, ReliabilityModes.ReliableOrdered);
            endEvt.Row = finalSelected.x;
            endEvt.Column = finalSelected.y;
            endEvt.Send();

            await Task.Delay(1000);
            var nextNode = DungeonModel.Instance.NodeMapModel.MapData.Nodes.Find(x => x.Pos == finalSelected);
            var sceneName = DungeonModel.Instance.StagePoolModel.Next(nextNode.Type);
            state.StageState = CreateStageToken(nextNode.Pos.x, sceneName);
            if (BoltServerCallbacks.IsDedicated)
                DungeonModel.Instance.NodeMapModel.SelectNextNode(nextNode.Pos);
        }

        public void NodeSelected(BoltConnection connection, Vector2Int pos)
        {
            waitConnections.Remove(connection);
            if (selectedCount.ContainsKey(pos))
                selectedCount[pos]++;
        }

        public void NodeSkipped(BoltConnection connection)
        {
            waitConnections.Remove(connection);
        }


        private StageStateToken CreateStageToken(int idx, string sceneName)
        {
            var token = ProtocolTokenUtils.GetToken<StageStateToken>();
            token.Idx = idx;
            token.SceneName = sceneName;
            return token;
        }
    }
}