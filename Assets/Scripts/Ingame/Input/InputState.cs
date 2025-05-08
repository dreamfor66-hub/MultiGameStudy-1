using FMLib.Structs;

namespace Rogue.Ingame.Input
{
    public readonly struct InputState
    {
        public int Direction { get; }
        public int Buttons { get; }

        public VectorXZ DirectionVec => DirectionEncoder.Decode(Direction);

        public bool Basic => (Buttons & BasicMask) != 0;
        public bool Special => (Buttons & SpecialMask) != 0;
        public bool Evade => (Buttons & EvadeMask) != 0;
        public bool Ultimate => (Buttons & UltimateMask) != 0;
        public bool SkillA => (Buttons & skillAMask) != 0;
        public bool SkillS => (Buttons & skillSMask) != 0;
        public bool SkillD => (Buttons & skillDMask) != 0;

        private const int BasicMask = 1 << 0;
        private const int SpecialMask = 1 << 1;
        private const int EvadeMask = 1 << 2;
        private const int UltimateMask = 1 << 3;
        private const int skillAMask = 1 << 4;
        private const int skillSMask = 1 << 5;
        private const int skillDMask = 1 << 6;

        public InputState(float horizontal, float vertical, bool basic, bool special, bool evade, bool ultimate, bool skillA, bool skillS, bool skillD)
        {
            Direction = DirectionEncoder.Encode(new VectorXZ(horizontal, vertical));
            Buttons = (basic ? BasicMask : 0) +
                      (special ? SpecialMask : 0) +
                      (evade ? EvadeMask : 0) +
                      (ultimate ? UltimateMask : 0) +
                      (skillA ? skillAMask : 0) +
                      (skillS ? skillSMask : 0) +
                      (skillD ? skillDMask : 0);
        }

        public InputState(int direction, int buttons)
        {
            Direction = direction;
            Buttons = buttons;
        }


        public static bool operator ==(InputState left, InputState right)
        {
            return left.Direction == right.Direction && left.Buttons == right.Buttons;
        }
        public static bool operator !=(InputState left, InputState right)
        {
            return !(left == right);
        }
    }
}