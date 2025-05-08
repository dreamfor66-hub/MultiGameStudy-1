namespace Rogue.Ingame.Character
{
    public enum CharacterStateType
    {
        Idle,
        Run,
        WalkAround,
        Hurt,
        Dead,
        Stun,
        Action,
    }

    public enum CharacterCommandType
    {
        None = 0,
        BasicAttack = 1,
        SpecialAttack = 2,
        Evade = 3,
        Ultimate = 4,

        SkillA = 5,
        SkillS = 6,
        SkillD = 7,

        BasicAttackUp = 11,
        BasicAttackDown = 12,

        SpecialAttackUp = 21,
        SpecialAttackDown = 22,

        //SkillAUp = 51,
        //SkillADown = 52,

        //SkillSUp = 61,
        //SkillSDown = 62,

        //SkillDUp = 71,
        //SkillDDown = 72,
    }
}