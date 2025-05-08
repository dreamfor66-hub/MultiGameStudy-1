namespace Rogue.Ingame.Goods
{
    public class ItemBuyModel
    {
        private readonly MoneyModel moneyModel;
        private readonly ItemInfoModel itemInfoModel;

        public ItemBuyModel(MoneyModel moneyModel, ItemInfoModel itemInfoModel)
        {
            this.moneyModel = moneyModel;
            this.itemInfoModel = itemInfoModel;
        }
        public bool Buy(ShopItemType itemType)
        {
            var price = itemInfoModel.GetPrice(itemType);
            if (moneyModel.Amount >= price)
            {
                moneyModel.Use(price);
                return true;
            }
            else
                return false;
        }
    }
}