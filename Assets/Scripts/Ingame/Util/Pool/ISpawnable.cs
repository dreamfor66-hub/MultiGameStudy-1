using System;

namespace Rogue.Ingame.Util.Pool
{
    public interface ISpawnable
    {
        void RegisterDespawn(Action action);
        void OnSpawn();
        void OnDespawn();
    }
}