using System;
using System.Linq;
using FMLib.Extensions;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Character
{
    public class CharacterSpawn
    {
        private readonly GameObject me;
        private readonly Transform tm;

        public CharacterSpawn(GameObject me)
        {
            this.me = me;
            tm = me.transform;
        }

        public void UpdateSpawn(CharacterStateUpdateInfo updateInfo)
        {
            if (updateInfo.Cur.StateType == CharacterStateType.Action)
            {
                var actionData = updateInfo.Cur.ActionData;
                var curFrame = updateInfo.Cur.Frame;
                var prevFrame = curFrame - updateInfo.DeltaFrame;
                UpdateAction(actionData, prevFrame, curFrame);

            }
        }

        private void UpdateAction(ActionData actionData, float prevFrame, float curFrame)
        {
            foreach (var spawnData in actionData.MonsterSpawnData)
            {
                if (prevFrame < spawnData.Frame && spawnData.Frame <= curFrame)
                {
                    Spawn(spawnData);
                }
            }
        }


        private void Spawn(MonsterSpawnData data)
        {
            var anchorPosition = GetPositionAnchor(data);
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


        // TODO : MonsterSpawner.cs 와 중복 
        private Vector3 GetPositionAnchor(MonsterSpawnData data)
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