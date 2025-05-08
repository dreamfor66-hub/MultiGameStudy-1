using System.Collections.Generic;
using FMLib.Structs;
using Photon.Bolt;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [System.Serializable]
    public class StageWaveConditionData
    {
        public int RemainMonster;
    }

    [System.Serializable]
    public class MonsterSpawnData
    {
        [TableColumnWidth(40, false)]
        public int Frame;

        public GameObject MonsterPrefab;

        [TableColumnWidth(120, false)]
        public SpawnPositionType PositionType;

        [TableColumnWidth(90, false)]
        [ShowIf(nameof(PositionType), SpawnPositionType.SpawnPoint)]
        public Transform SpawnPoint;

        [TableColumnWidth(60, false)]
        public VectorXZ PosMin;

        [TableColumnWidth(60, false)]
        public VectorXZ PosMax;


        [TableColumnWidth(40, false)]
        public int Count = 1;
    }

    public enum SpawnPositionType
    {
        World,
        Player,
        Spawner,
        SpawnPoint,
    }

    [System.Serializable]
    public class StageWaveData
    {
        [Title("Monsters")]
        [TableList]
        [HideLabel]
        public List<MonsterSpawnData> Monsters;

        [Title("Condition")]
        [HideLabel]
        public StageWaveConditionData NextCondition;
    }
}