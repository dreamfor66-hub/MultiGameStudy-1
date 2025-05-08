using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMLib.Random;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Event;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Rogue.Ingame.Debugger
{
    [Serializable]
    public class EntityBuffList
    {
        public IEntity Entity;
        public List<BuffData> Buffs = new List<BuffData>();
    }

    public class BuffDebugger : MonoBehaviour
    {
        [TableList] [ShowInInspector] private List<EntityBuffList> entities = new List<EntityBuffList>();
        public bool ShowOnGui;


        private void Start()
        {
            EntityTable.Entities.ForEach(Register);
            EntityTable.OnAdd += Register;
            EntityTable.OnRemove += Remove;
            EventDispatcher.Listen<EventChangeBuff>(OnEventChangeBuff);
        }

        private void OnDestroy()
        {
            EntityTable.OnAdd -= Register;
            EntityTable.OnRemove -= Remove;
            EventDispatcher.Remove<EventChangeBuff>(OnEventChangeBuff);
        }

        private void OnGUI()
        {
            if (!ShowOnGui)
                return;

            var sb = new StringBuilder();
            foreach (var e in entities)
            {
                sb.Append($"{e.Entity.GameObject.name}\n");
                foreach (var buff in e.Buffs)
                    sb.Append($"\tㄴ{buff.name}\n");
            }
            GUI.Label(new Rect(0, 0, 1920, 1080), sb.ToString());
        }

        private void Register(IEntity entity)
        {
            if (entity is CharacterBehaviour character)
            {
                var ebl = new EntityBuffList();
                ebl.Entity = character;
                ebl.Buffs.AddRange(character.BuffAccepter.GetBuffs().Select(x => x.Data));
                entities.Add(ebl);
            }
        }

        private void Remove(IEntity entity)
        {
            entities.RemoveAll(x => x.Entity == entity);
        }

        private void OnEventChangeBuff(EventChangeBuff evt)
        {
            Debug.Log(
                $"<color=yellow>[Buff]</color> {(evt.IsAdd ? "Add" : "Remove")} {evt.Entity.GameObject.name} > {evt.Buff.name}");

            var find = entities.Find(x => x.Entity == evt.Entity);
            if (find != null)
                if (evt.IsAdd)
                    find.Buffs.Add(evt.Buff);
                else
                    find.Buffs.Remove(evt.Buff);
        }

    }
}
