using System.Collections.Generic;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Character
{
    public class HitboxGroupMemory
    {
        private readonly Dictionary<int, List<IEntity>> dict;

        public HitboxGroupMemory()
        {
            dict = new Dictionary<int, List<IEntity>>();
        }

        private List<IEntity> GetList(int groupId)
        {
            if (!dict.ContainsKey(groupId))
                dict.Add(groupId, new List<IEntity>());
            return dict[groupId];
        }

        public bool Contains(int groupId, IEntity entity)
        {
            return GetList(groupId).Contains(entity);
        }

        public int GetCount(int groupId)
        {
            return GetList(groupId).Count;
        }

        public void Add(int groupId, IEntity entity)
        {
            GetList(groupId).Add(entity);
        }

        public void Clear()
        {
            foreach (var hurts in dict.Values)
            {
                hurts.Clear();
            }
        }

    }
}