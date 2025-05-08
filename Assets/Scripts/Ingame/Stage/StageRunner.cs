using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Random;
using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Stage
{
    public class StageRunner : MonoBehaviour
    {
        public IReadOnlyList<StageWaveData> Waves => waves;

        [SerializeField] public List<StageWaveData> waves;
        [Button]
        public void CopyFromData()
        {
            waves = new List<StageWaveData>(waves);
        }

        public Vector3 Vec3;
        public VectorXZ VecXz;

        private int waveIdx;

        private MonsterSpawner spawner;
        private float elapsedTime = 0f;

        public Action OnClear;

        private StageValueData stageValueData;
        private NodeTypeBuffData nodeTypeBuffData;
        private float playerCountHpBuff = 1f;
        private readonly RandomSelectNFromM eliteSelector = new RandomSelectNFromM();
        private readonly RandomSelectNFromM randomBuffSelector = new RandomSelectNFromM();
        private Func<GameObject, Vector3, Quaternion, GameObject> spawnAction;
        private bool isInit;

        public void Init(StageValueData stageValueData, NodeTypeBuffData nodeTypeBuffData, float playerCountHpBuff, Func<GameObject, Vector3, Quaternion, GameObject> spawnAction)
        {
            this.stageValueData = stageValueData;
            this.nodeTypeBuffData = nodeTypeBuffData;
            this.playerCountHpBuff = playerCountHpBuff;
            this.spawnAction = spawnAction;
            var totalCount = waves.Sum(x => x.Monsters.Sum(y => y.Count));
            var eliteCount = Random.Range(nodeTypeBuffData.EliteCountMinMax.x, nodeTypeBuffData.EliteCountMinMax.y + 1);

            eliteSelector.Init(eliteCount, totalCount);
            isInit = true;
        }

        private void Start()
        {
            spawner = new MonsterSpawner(this.transform, Spawn);
            Reset();
        }

        private void Reset()
        {
            waveIdx = -1;
            NextWave();
        }

        private void NextWave()
        {
            waveIdx++;
            if (waveIdx < waves.Count)
            {
                var curWave = waves[waveIdx];
                spawner.Reset(curWave.Monsters);
            }
            else
            {
                OnClear?.Invoke();
                spawner.Clear();
            }
            elapsedTime = 0f;
        }

        private void Update()
        {
            if (!isInit)
                return;
            elapsedTime += Time.deltaTime;
            spawner.Simulate(elapsedTime, int.MaxValue);

            if (spawner.IsDone && CheckNextCondition())
            {
                NextWave();
            }
        }

        private bool CheckNextCondition()
        {
            if (waveIdx >= waves.Count)
                return false;
            var curWave = waves[waveIdx];
            return CheckCondition(curWave.NextCondition);
        }

        private bool CheckCondition(StageWaveConditionData condition)
        {
            var monsters = EntityTable.Entities.Where(x => x.Team == Team.Monster && x is CharacterBehaviour);
            var remainCount = monsters.Count();
            return remainCount <= condition.RemainMonster;
        }

        private void Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var instance = spawnAction(prefab, position, rotation);

            var character = instance.GetComponent<CharacterBehaviour>();
            character.HpModule.SetPlayerCountHpBuff(playerCountHpBuff);
            character.BuffValues.AddValue(BuffSimpleValueType.AttackDamagePercent, stageValueData.AttackPercent);
            character.BuffValues.AddValue(BuffSimpleValueType.MaxHpPercent, stageValueData.HpPercent);

            if (eliteSelector.Pick())
            {
                foreach (var fixedBuff in nodeTypeBuffData.EliteFixedBuffs)
                {
                    character.BuffAccepter.AddBuff(fixedBuff, character);
                }

                var randomBuffCount = Random.Range(nodeTypeBuffData.RandomBuffCountMinMax.x,
                    nodeTypeBuffData.RandomBuffCountMinMax.y + 1);

                var randomBuffs = nodeTypeBuffData.EliteRandomBuffs.SelectN(RandomByUnity.Instance, randomBuffCount);
                foreach (var buff in randomBuffs)
                {
                    character.BuffAccepter.AddBuff(buff, character);
                }
            }
        }

    }
}
