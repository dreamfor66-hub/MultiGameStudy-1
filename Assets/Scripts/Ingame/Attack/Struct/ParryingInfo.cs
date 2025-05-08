using FMLib.Structs;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Attack.Struct
{
    public struct ParryingInfo
    {
        public IEntity Attacker;
        public IEntity Victim;
        public HitFxData HitFx;
        public VectorXZ Direction;
    }
}