using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rogue.Ingame.Character;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Bot
{
    public class CharacterAggro
    {
        public CharacterBehaviour Character;
        public float Aggro;
    }

    [Serializable]
    public class AggroVariables
    {
        public float AggroFirstTime = 200f;
        public float AggroByPlayerHitMonster = 6f;
        public float AggroByPlayerHitMonsterWhileActive = 9f;
        public float AggroByReviveChannelingPerSec = 40f;
        public float AggroByMonsterHitPlayer = -10f;
        public float DecayByAction = 25f;
        public float AggroLazyCap = 100f;
        public float AggroInactionCap = 40f;
        public float AggroByLazy = 40f;
        public float AggroByInaction = 140f;
        public float AggroByTaunt = 60f;
    }

    public class AggroTable
    {
        private static readonly AggroVariables defaultVariables = new AggroVariables
        {
            AggroFirstTime = 200f,
            AggroByPlayerHitMonster = 6f,
            AggroByPlayerHitMonsterWhileActive = 9f,
            AggroByReviveChannelingPerSec = 40f,
            AggroByMonsterHitPlayer = -10f,
            DecayByAction = 25f,
            AggroLazyCap = 100f,
            AggroInactionCap = 40f,
            AggroByLazy = 40f,
            AggroByInaction = 140f,
            AggroByTaunt = 60f
        };

        private readonly List<CharacterAggro> table;
        private readonly AggroVariables variables = defaultVariables;

        public AggroTable(AggroVariables variables)
        {
            if (variables != null)
                this.variables = variables;

            table = EntityTable.Entities.OfType<CharacterBehaviour>().Where(x => x.Team == Team.Player).Select(x => new CharacterAggro
            {
                Character = x,
                Aggro = Mathf.Clamp(this.variables.AggroFirstTime, 0f, float.MaxValue)
            }).ToList();
        }

        public void Hurt(CharacterBehaviour attacker)
        {
            Change(attacker, variables.AggroByPlayerHitMonster);
        }

        public void HurtWhileActive(CharacterBehaviour attacker)
        {
            Change(attacker, variables.AggroByPlayerHitMonsterWhileActive);
        }

        public void UpdateFrame(float time)
        {
            table.RemoveAll(x => x.Character == null);
            table.ForEach(x =>
            {
                if (IsReviveChanneling(x.Character))
                    x.Aggro += Mathf.Clamp(variables.AggroByReviveChannelingPerSec * time, 0f, float.MaxValue);
            });
        }

        public void DoPattern(IEntity target, int reduceAggro)
        {
            table.ForEach(x =>
            {
                x.Aggro = Mathf.Clamp(x.Aggro - variables.DecayByAction, 0f, float.MaxValue);
            });
            if (target is CharacterBehaviour character)
                Change(character, -reduceAggro);

        }

        public void HitTarget(CharacterBehaviour target)
        {
            Change(target, variables.AggroByMonsterHitPlayer);
        }

        public void BuffGainAggro(CharacterBehaviour target, float value)
        {
            Change(target, value);
        }

        public void Taunt(CharacterBehaviour target)
        {
            Change(target, table.Select(x => x.Aggro).Max() + variables.AggroByTaunt - GetAggro(target));
        }

        private void Change(CharacterBehaviour character, float diff)
        {
            if (character.team != Team.Player)
                return;
            var find = table.Find(x => x.Character == character);
            if (find == null)
            {
                table.Add(new CharacterAggro
                {
                    Aggro = Mathf.Clamp(variables.AggroFirstTime + diff, 0f, float.MaxValue),
                    Character = character
                });
            }
            else
            {
                if (find.Aggro < variables.AggroLazyCap)
                    find.Aggro += Mathf.Clamp(find.Aggro + variables.AggroByLazy, 0f, float.MaxValue);
                find.Aggro = Mathf.Clamp(find.Aggro + diff, 0f, float.MaxValue);
            }

            foreach (var elem in table)
            {
                if (elem.Aggro < variables.AggroInactionCap)
                    elem.Aggro += Mathf.Clamp(elem.Aggro + variables.AggroByInaction, 0f, float.MaxValue);
            }
        }

        public float GetAggro(CharacterBehaviour character)
        {
            var find = table.Find(x => x.Character == character);
            return find?.Aggro ?? 0f;
        }

        public bool IsReviveChanneling(CharacterBehaviour character)
        {
            if (character.Character.StateInfo.StateType != CharacterStateType.Idle)
                return false;

            var deadPlayers = EntityTable.Entities.OfType<CharacterBehaviour>()
                .Where(x => x.Team == Team.Player && x.IsDead);
            return deadPlayers.Any(x => Vector3.Distance(x.transform.position, character.transform.position) < 2.5f);
        }

        public string GetDebugString()
        {
            var sb = new StringBuilder();
            var sortedTable = table.ToList();
            sortedTable.OrderByDescending(x => x.Aggro);
            foreach (var aggro in sortedTable)
            {
                if (aggro.Aggro > sortedTable[0].Aggro - 20)
                    sb.Append($"<color=red>[{aggro.Character.EntityId}] {aggro.Character.transform.name} : {aggro.Aggro:0.000}</color>\n");
                else if (aggro.Aggro > sortedTable[0].Aggro - 80)
                    sb.Append($"<color=yellow>[{aggro.Character.EntityId}] {aggro.Character.transform.name} : {aggro.Aggro:0.000}</color>\n");
                else
                    sb.Append($"<color=grey>[{aggro.Character.EntityId}] {aggro.Character.transform.name} : {aggro.Aggro:0.000}</color>\n");
            }

            return sb.ToString();
        }
    }
}