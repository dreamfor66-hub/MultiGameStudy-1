using System;
using FMLib.UI.OnOff;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Event;
using Rogue.Ingame.GameCommand;
using Rogue.Ingame.Goods;
using Rogue.Ingame.Input;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    public class ShopItemArea : MonoBehaviour
    {
        [SerializeField] [Required] private OnOffBehaviour buyableOnOff;
        [SerializeField] [Required] private OnOffBehaviour activeOnOff;
        [SerializeField] private ShopItemType itemType;
        [SerializeField] [Required] private TextMeshProUGUI priceText;
        [SerializeField] [Required] private OnOffBehaviour purchasedOnOff;

        private bool isBuyable = false;
        private int price = 0;
        private void Start()
        {
            SetActive(true);
            SetBuyable(false);
            SetPurchased(false);

            price = 0;
            //GoodsModel.Instance.ItemInfoModel.GetPrice(this.itemType);
            priceText.text = price.ToString();

            if (!GoodsModel.Instance.ItemInfoModel.CanBuy(this.itemType))
                Destroy(gameObject);
        }

        private void Update()
        {
            if (isBuyable && InputDetector.GetConfirmButton())
                Buy();
        }

        private void SetBuyable(bool buyable)
        {
            isBuyable = buyable;
            buyableOnOff.Set(buyable);
        }

        private void Buy()
        {
            //if (GoodsModel.Instance.ItemBuyModel.Buy(itemType))
            //{
                BuyAction(itemType);
                EventDispatcher.Send(new EventCoinGet(-price, OwnerCharacterHolder.OwnerCharacter));
                SetActive(false);
                SetBuyable(false);
                SetPurchased(true);
            //}
        }

        private void SetActive(bool active)
        {
            activeOnOff.Set(active);
        }

        private void SetPurchased(bool active)
        {
            purchasedOnOff.Set(active);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == OwnerCharacterHolder.OwnerCharacter.gameObject)
                SetBuyable(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == OwnerCharacterHolder.OwnerCharacter.gameObject)
                SetBuyable(true);
        }

        private static async void BuyAction(ShopItemType itemType)
        {
            switch (itemType)
            {
                case ShopItemType.PerkSlot:
                    RewardModel.Instance.SlotCountModel.AddSlot();
                    break;
                case ShopItemType.RandomPerk:
                    await RewardManager.Instance.Reward(3);
                    break;
                case ShopItemType.StatBuff:
                    break;
                case ShopItemType.Recovery:
                    GoodsModel.Instance.ItemBuffModel.AddBuff(RewardModel.Instance.RewardTable.Heal30Buff);
                    break;
                case ShopItemType.Revive:
                    GoodsModel.Instance.ItemBuffModel.AddBuff(RewardModel.Instance.RewardTable.ReviveBuff);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }
    }
}
