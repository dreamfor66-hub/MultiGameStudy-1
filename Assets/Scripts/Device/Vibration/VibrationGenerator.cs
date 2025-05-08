using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Rogue.Device.Vibration
{
    public static class VibrationGenerator
    {
        public static void High(float time)
        {
            Vibrate(0f, 1f, time);
        }

        public static void Vibrate(float low, float high, float time)
        {
            foreach (var pad in Gamepad.all)
            {
                Vibrate(pad, low, high, time);
            }
        }

        private static async void Vibrate(Gamepad pad, float low, float high, float time)
        {
            pad.SetMotorSpeeds(low, high);
            await Task.Delay((int)(time * 1000));
            pad.SetMotorSpeeds(0, 0);
        }
    }
}