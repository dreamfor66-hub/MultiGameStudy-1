using System;
using System.Linq;
using Rogue.Ingame.Character;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Reward;

namespace Rogue.Ingame.Goods
{
    public class ItemInfoModel
    {
        public bool CanBuy(ShopItemType itemType)
        {
            switch (itemType)
            {
                case ShopItemType.PerkSlot:
                    return RewardModel.Instance.SlotCountModel.SlotCount < 10;
                case ShopItemType.RandomPerk:
                    return true;
                case ShopItemType.StatBuff:
                    return true;
                case ShopItemType.Recovery:
                    return true;
                case ShopItemType.Revive:
                    return EntityTable.Entities.Where(x =>
                    {
                        if (x.Team != Team.Player)
                            return false;
                        if (x is CharacterBehaviour character)
                            return character.IsDead;
                        return false;
                    }).Any();
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }
        public int GetPrice(ShopItemType itemType)
        {
            switch (itemType)
            {
                case ShopItemType.PerkSlot:
                    var slotCount = RewardModel.Instance.SlotCountModel.SlotCount;
                    if (slotCount <= 7) return 300;
                    else if (slotCount == 8) return 500;
                    else if (slotCount == 9) return 700;
                    else return 1000;
                case ShopItemType.RandomPerk:
                    return 300;
                case ShopItemType.StatBuff:
                    return 300;
                case ShopItemType.Recovery:
                    return 200;
                case ShopItemType.Revive:
                    return 400;
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }
    }
}