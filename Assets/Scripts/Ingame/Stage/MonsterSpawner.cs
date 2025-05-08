using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Extensions;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Stage
{
    public class MonsterSpawner
    {
        private List<MonsterSpawnData> spawnList;
        private int spawnIdx = 0;

        private readonly Transform spawnerTm;
        private readonly Action<GameObject, Vector3, Quaternion> spawnAction;

        public bool IsDone => spawnList == null || spawnIdx >= spawnList.Count;
        public MonsterSpawner(Transform spawner, Action<GameObject, Vector3, Quaternion> spawnAction)
        {
            spawnerTm = spawner;
            this.spawnAction = spawnAction;
        }

        public void Reset(List<MonsterSpawnData> list)
        {
            spawnList = list;
            spawnIdx = 0;
        }

        public void Clear()
        {
            Reset(null);
        }

        public void Simulate(float elapsedTime, int limit)
        {
            if (spawnList == null)
                return;

            var count = 0;
            while (spawnIdx < spawnList.Count)
            {
                var curMonster = spawnList[spawnIdx];
                var curTime = curMonster.Frame / (float)CommonVariables.GameFrame;
                if (curTime <= elapsedTime)
                {
                    Debug.Log($"Spawn-{curMonster.MonsterPrefab.name}, {curTime}, {elapsedTime}");
                    if (count < limit)
                        Spawn(curMonster);
                    count++;
                    spawnIdx++;
                }
                else
                {
                    break;
                }
            }
        }

        private void Spawn(MonsterSpawnData spawnData)
        {
            var anchorPosition = GetPositionAnchor(spawnData);
            for (var i = 0; i < spawnData.Count; i++)
            {
                var pos = VectorExtensions.Random(spawnData.PosMin, spawnData.PosMax) + anchorPosition;
                Spawn(spawnData.MonsterPrefab, pos);
            }
        }

        private void Spawn(GameObject prefab, Vector3 position)
        {
            var angle = Random.Range(-180, 180);
            var rot = Quaternion.AngleAxis(angle, Vector3.up);
            if (NavMesh.SamplePosition(position, out var hit, 10f, NavMesh.AllAreas))
            {
                position = hit.position;
            }

            spawnAction?.Invoke(prefab, position, rot);
        }

        private Vector3 GetPositionAnchor(MonsterSpawnData data)
        {
            var type = data.PositionType;
            switch (type)
            {
                case SpawnPositionType.World:
                    return Vector3.zero;
                case SpawnPositionType.Spawner:
                    return spawnerTm.position;
                case SpawnPositionType.Player:
                    var players = EntityTable.Entities.Where(x => x.Team == Team.Player && x is CharacterBehaviour).ToArray();
                    if (players.Length == 0)
                    {
                        return Vector3.zero;
                    }
                    else
                    {
                        var idx = Random.Range(0, players.Length);
                        return players[idx].GameObject.transform.position;
                    }
                case SpawnPositionType.SpawnPoint:
                    return data.SpawnPoint != null ? data.SpawnPoint.position : Vector3.zero;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}