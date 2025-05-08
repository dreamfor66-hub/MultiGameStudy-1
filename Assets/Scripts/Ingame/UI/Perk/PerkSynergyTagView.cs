using Rogue.Ingame.Reward;
using Rogue.Ingame.UI.Perk;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace FMLib.UI.OnOff
{
    public class PerkSynergyTagView : OnOffActiveSelf
    {
        [SerializeField] [Required] private Text Text;
        [SerializeField] [Required] private SynergyIconView Icon;


        public void Set(bool onOff, SynergyData synergy)
        {
            if (onOff)
            {
                Text.text = synergy.Name;
                Icon.Set(synergy, -1);
            }
            base.Set(onOff);
        }
    }
}