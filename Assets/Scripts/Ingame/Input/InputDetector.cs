using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Rogue.Ingame.Input
{
    public enum ControllerType
    {
        KeyboardMouse = 0,
        Pad = 1
    }

    public static class InputDetector
    {
        private static readonly string[] horizontal = { "Horizontal-KM", "Horizontal-Pad1" };
        private static readonly string[] vertical = { "Vertical-KM", "Vertical-Pad1" };
        private static readonly string[] basic = { "Attack1-KM", "Attack1-Pad1" };
        private static readonly string[] special = { "Attack2-KM", "Attack2-Pad1" };
        private static readonly string[] evade = { "Evade-KM", "Evade-Pad1" };
        private static readonly string[] ultimate = { "Special-KM", "Special-Pad1" };
        private static readonly string[] skillA = { "SkillA-KM", "SkillA-Pad1" };
        private static readonly string[] skillS = { "SkillS-KM", "SkillS-Pad1" };
        private static readonly string[] skillD = { "SkillD-KM", "SkillD-Pad1" };
        private static readonly string[] select = { "Select-KM", "Select-Pad1" };
        private static readonly string[] confirm = { "Confirm-KM", "Confirm-Pad1" };

        private const int KeyboardMouse = 0;
        private const int Pad1 = 1;

        private static int curIdx = 0;
        private static int lastFrame = 0;

        public static ControllerType CurController => (ControllerType)curIdx;

        private static IEnumerable<string> ButtonList(int idx)
        {
            yield return basic[idx];
            yield return special[idx];
            yield return evade[idx];
            yield return ultimate[idx];
            yield return skillA[idx];
            yield return skillS[idx];
            yield return skillD[idx];
            yield return select[idx];
            yield return confirm[idx];
        }

        private static IEnumerable<string> AxisList(int idx)
        {
            yield return horizontal[idx];
            yield return vertical[idx];
        }


        private static void UpdateIdx()
        {
            if (lastFrame == Time.frameCount)
                return;
            lastFrame = Time.frameCount;

            CheckIdx(Pad1);
            CheckIdx(KeyboardMouse);
        }

        private static void CheckIdx(int idx)
        {
            if (ButtonList(idx).Any(UnityInput.GetButton))
                curIdx = idx;

            if (AxisList(idx).Any(x => Mathf.Abs(UnityInput.GetAxis(x)) > 0.3f))
                curIdx = idx;
        }

        public static float GetHorizontal()
        {
            UpdateIdx();
            return UnityInput.GetAxis(horizontal[curIdx]);
        }

        public static float GetVertical()
        {
            UpdateIdx();
            return UnityInput.GetAxis(vertical[curIdx]);
        }

        public static bool GetOKButton()
        {
            UpdateIdx();
            return UnityInput.GetButtonDown(basic[curIdx]) || UnityInput.GetButtonDown(evade[curIdx]);
        }

        public static bool GetSelectLButton()
        {
            UpdateIdx();
            return UnityInput.GetButtonDown(select[curIdx]);
        }

        public static bool GetConfirmButton()
        {
            UpdateIdx();
            return UnityInput.GetButtonDown(confirm[curIdx]);
        }

        public static InputState GetState()
        {
            UpdateIdx();

            return new InputState(
                UnityInput.GetAxis(horizontal[curIdx]),
                UnityInput.GetAxis(vertical[curIdx]),
                UnityInput.GetButton(basic[curIdx]),
                UnityInput.GetButton(special[curIdx]),
                UnityInput.GetButton(evade[curIdx]),
                UnityInput.GetButton(ultimate[curIdx]),
                UnityInput.GetButton(skillA[curIdx]),
                UnityInput.GetButton(skillS[curIdx]),
                UnityInput.GetButton(skillD[curIdx])
            );
        }
    }
}