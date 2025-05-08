using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.Ingame.Entity
{
    public static class EntityTable
    {
        public static int IdNotDefined = 0;
        private static int idGen = 1;

        public static IReadOnlyList<IEntity> Entities => entities;
        private static readonly List<IEntity> entities = new List<IEntity>();
        public static Action<IEntity> OnAdd;
        public static Action<IEntity> OnRemove;

        public static int GetId()
        {
            return idGen++;
        }

        public static void Add(IEntity entity)
        {
            entities.Add(entity);
            OnAdd?.Invoke(entity);
        }

        public static void Remove(IEntity entity)
        {
            entities.Remove(entity);
            OnRemove?.Invoke(entity);
        }

        public static IEntity FindById(int id)
        {
            return entities.Find(x => x.EntityId == id);
        }

        public static List<T> FindByType<T>() where T : IEntity
        {
            var Values = entities.OfType<T>().ToList();
            return Values;
        }

        public static List<T> FindEntitiesByTeam<T>(Team team) where T : IEntity
        {
            var Values = entities.OfType<T>().Where(x=>x.Team == team).ToList();
            return Values;
        }
    }
}