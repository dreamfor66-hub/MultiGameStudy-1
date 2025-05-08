using System;
using FMLib.UI.OnOff;
using Rogue.Ingame.Character;
using Rogue.Ingame.Event;
using Rogue.Ingame.Goods;
using Rogue.Ingame.Input;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    public class RecoveryArea : MonoBehaviour
    {
        [SerializeField] [Required] private OnOffBehaviour interactableOnOff;
        [SerializeField] [Required] private OnOffBehaviour activeOnOff;

        private bool isInteractable = false;

        private void Start()
        {
            SetActive(true);
            SetInteractable(false);
        }

        private void Update()
        {
            if (isInteractable && InputDetector.GetConfirmButton())
                Interact();
        }

        private void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
            interactableOnOff.Set(interactable);
        }

        private void Interact()
        {
            SetActive(false);
            SetInteractable(false);
            GoodsModel.Instance.ItemBuffModel.AddBuff(RewardModel.Instance.RewardTable.Heal30Buff);
        }

        private void SetActive(bool active)
        {
            activeOnOff.Set(active);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == OwnerCharacterHolder.OwnerCharacter.gameObject)
                SetInteractable(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == OwnerCharacterHolder.OwnerCharacter.gameObject)
                SetInteractable(true);
        }

        private static async void BuyAction(ShopItemType itemType)
        {
            switch (itemType)
            {
                case ShopItemType.PerkSlot:
                    RewardModel.Instance.SlotCountModel.AddSlot();
                    break;
                case ShopItemType.RandomPerk:
                    await RewardManager.Instance.Reward(0);
                    break;
                case ShopItemType.StatBuff:
                    break;
                case ShopItemType.Recovery:

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