using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI
{
    public class CoolTimeSingleUI : MonoBehaviour
    {
        [SerializeField] private Image filledImage;

        public void SetCoolTime(int remainFrame, int coolFrame)
        {
            filledImage.fillAmount = 1 - (float)remainFrame / coolFrame;
        }
    }
}