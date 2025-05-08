using Rogue.Ingame.Data;

namespace Rogue.Ingame.Character
{
    public interface IActionResource
    {
        public bool CanUse(int count);
        public void Use(int count);
    }


    public struct StaminaInfo
    {
        public int CurStamina;
        public int NextFrame;
    }

    public class StaminaModule : IActionResource
    {
        public int Max => maxStamina;
        public int Cur => curStamina;
        public int ChargingFrame => staminaData.ChargingFramePerStamina;
        public float Ratio => 1 - (float)nextFrame / ChargingFrame;


        public StaminaInfo Info => new StaminaInfo
        {
            CurStamina = curStamina,
            NextFrame = nextFrame,
        };

        private readonly StaminaData staminaData;
        private int maxStamina;
        private int curStamina;
        private int nextFrame;

        public StaminaModule(StaminaData staminaData)
        {
            this.staminaData = staminaData;
            maxStamina = staminaData.BasicMaxStamina;
            curStamina = maxStamina;
            nextFrame = staminaData.ChargingFramePerStamina;
        }

        public void Sync(StaminaInfo info)
        {
            curStamina = info.CurStamina;
            nextFrame = info.NextFrame;
        }

        public void UpdateFrame()
        {
            if (curStamina < maxStamina)
            {
                nextFrame--;
                if (nextFrame <= 0)
                {
                    curStamina++;
                    nextFrame = staminaData.ChargingFramePerStamina;
                }
            }
        }

        public bool CanUse(int count)
        {
            return count <= curStamina;
        }

        public void Use(int count)
        {
            curStamina -= count;
        }
    }
}