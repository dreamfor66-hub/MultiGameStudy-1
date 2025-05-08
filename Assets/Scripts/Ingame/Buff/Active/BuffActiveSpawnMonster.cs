using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Extensions;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveSpawnMonster : IBuffActive
    {
        private readonly List<MonsterSpawnData> monsters;

        public BuffActiveSpawnMonster(BuffActiveData data)
        {
            this.monsters = data.Monsters;
        }
        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            foreach (var monster in monsters)
            {
                Spawn(monster, target.GameObject.transform);
            }
        }

        //TODO: CharacterSpawn.cs와 중복
        private void Spawn(MonsterSpawnData data, Transform tm)
        {
            var anchorPosition = GetPositionAnchor(data, tm);
            for (var i = 0; i < data.Count; i++)
            {
                var pos = VectorExtensions.Random(data.PosMin, data.PosMax) + anchorPosition;
                Spawn(data.MonsterPrefab, pos);
            }
        }

        private void Spawn(GameObject prefab, Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out var hit, 10f, NavMesh.AllAreas))
            {
                position = hit.position;
            }
            GameCommandSpawnMonster.Send(prefab, position);
        }

        private Vector3 GetPositionAnchor(MonsterSpawnData data, Transform tm)
        {
            var type = data.PositionType;
            switch (type)
            {
                case SpawnPositionType.World:
                    return Vector3.zero;
                case SpawnPositionType.Player:
                    var players = EntityTable.Entities.Where(x => x.Team == Team.Player && x is CharacterBehaviour)
                        .ToArray();
                    if (players.Length == 0)
                    {
                        return Vector3.zero;
                    }
                    else
                    {
                        var idx = Random.Range(0, players.Length);
                        return players[idx].GameObject.transform.position;
                    }
                case SpawnPositionType.Spawner:
                    return tm.position;
                case SpawnPositionType.SpawnPoint:
                    return data.SpawnPoint != null ? data.SpawnPoint.position : Vector3.zero;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}