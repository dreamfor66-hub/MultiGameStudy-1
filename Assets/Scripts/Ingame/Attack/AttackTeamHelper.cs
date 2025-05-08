using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Attack
{
    public static class AttackTeamHelper
    {
        public static bool Attackable(Team attacker, Team victim)
        {
            return victim == Team.None || attacker != victim;
        }

        public static bool Attackable(Team attacker, Hurtbox hurtbox)
        {
            return Attackable(attacker, hurtbox.Team);
        }

        public static bool IsTeam(Team a, Team b)
        {
            return a == b && a != Team.None;
        }

    }
}