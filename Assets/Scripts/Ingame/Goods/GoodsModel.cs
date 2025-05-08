using UnityEngine;

namespace Rogue.Ingame.Goods
{
    public class GoodsModel : MonoBehaviour
    {
        public static GoodsModel Instance { get; private set; }

        public MoneyModel MoneyModel;
        public ReRollModel ReRollModel;
        public ItemInfoModel ItemInfoModel;
        public ItemBuyModel ItemBuyModel;
        public ItemBuffModel ItemBuffModel;

        private void Awake()
        {
            Instance = this;
            Init();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Init()
        {
            MoneyModel = new MoneyModel();
            ReRollModel = new ReRollModel(MoneyModel);
            ItemInfoModel = new ItemInfoModel();
            ItemBuyModel = new ItemBuyModel(MoneyModel, ItemInfoModel);
            ItemBuffModel = new ItemBuffModel();
        }

        public void Reset()
        {
            MoneyModel.Reset();
            ReRollModel.Reset();
        }
    }
}