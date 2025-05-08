using System.Text;
using FMLib.UI.OnOff;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Reward.Struct;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class SynergySimpleView : MonoBehaviour
    {
        [SerializeField] [Required] private SynergyIconView synergyIcon;
        [SerializeField] [Required] private Text nameText;
        [SerializeField] [Required] private Text stepText;
        [SerializeField] [Required] private Text countText;
        [SerializeField] [Required] private OnOffBehaviour selectedOnOff;
        [SerializeField] [Required] private OnOffBehaviour detailOnOff;

        public void Set(SynergyChangeInfo info)
        {
            synergyIcon.Set(info.Synergy, info.CurLevel);
            nameText.text = info.Synergy.Name;
            stepText.text = StepText(info.Synergy, info.CurLevel, info.NextLevel);
            countText.text = info.CurCount.ToString();
            selectedOnOff.On();
        }

        public void Set(OwnSynergyInfo info)
        {
            synergyIcon.Set(info.Synergy, info.TriggerLevel);
            nameText.text = info.Synergy.Name;
            stepText.text = StepText(info.Synergy, info.TriggerLevel, info.TriggerLevel);
            countText.text = info.Count.ToString();
            selectedOnOff.Off();
        }

        [Button]
        public void ShowDetail(bool show)
        {
            detailOnOff.Set(show);
            if (show == false)
                selectedOnOff.Set(false);
        }

        private string StepText(SynergyData synergy, int grade, int nextGrade)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < synergy.Rewards.Count; i++)
            {
                var curNeed = synergy.Rewards[i].NeedCount;
                var gain = i > grade && i <= nextGrade;
                if (i == grade + 1 && i <= nextGrade)
                    sb.Append("<color=#00ff00>");


                if (i != 0)
                    sb.Append("  >  ");

                if (i == grade)
                {
                    sb.Append("<color=#ffffff>");
                    sb.Append(curNeed);
                    sb.Append("</color>");
                }
                else
                    sb.Append(curNeed);

                if (i > grade && i == nextGrade)
                    sb.Append("</color>");
            }
            return sb.ToString();
        }
    }
}