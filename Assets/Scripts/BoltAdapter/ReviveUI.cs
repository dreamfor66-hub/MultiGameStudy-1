using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.BoltAdapter
{
    public class ReviveUI : MonoBehaviour
    {
        [SerializeField] [Required] private TextMeshProUGUI text;

        private int prev = 0;
        private void Update()
        {
            if (BoltDungeonManager.Instance == null)
                return;
            var count = BoltDungeonManager.Instance.ReviveCount;
            if (count != prev)
            {
                text.text = count.ToString();
                prev = count;
            }
        }
    }
}