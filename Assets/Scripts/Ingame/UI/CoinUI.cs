using Rogue.Ingame.Goods;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class CoinUI : MonoBehaviour
    {
        [SerializeField] [Required] private TextMeshProUGUI text;

        private MoneyModel moneyModel => GoodsModel.Instance.MoneyModel;

        private void Start()
        {
            moneyModel.OnChanged += OnCoinChanged;
            OnCoinChanged();
        }

        private void Destroy()
        {
            moneyModel.OnChanged -= OnCoinChanged;
        }

        private void OnCoinChanged()
        {
            text.text = moneyModel.Amount.ToString();
        }
    }
}