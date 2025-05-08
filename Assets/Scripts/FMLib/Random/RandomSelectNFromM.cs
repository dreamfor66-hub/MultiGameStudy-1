namespace FMLib.Random
{
    public class RandomSelectNFromM
    {
        private int remain;
        private int remainSelect;

        public RandomSelectNFromM()
        {
        }

        public void Init(int select, int total)
        {
            remain = total;
            remainSelect = select;
        }

        public bool Pick()
        {
            if (remainSelect <= 0)
            {
                NotSelected();
                return false;
            }

            if (remainSelect >= remain)
            {
                Selected();
                return true;
            }

            if (UnityEngine.Random.value <= (float)remainSelect / remain)
            {
                Selected();
                return true;
            }
            else
            {
                NotSelected();
                return false;
            }
        }

        private void Selected()
        {
            remain--;
            remainSelect--;
        }

        private void NotSelected()
        {
            remain--;
        }
    }
}