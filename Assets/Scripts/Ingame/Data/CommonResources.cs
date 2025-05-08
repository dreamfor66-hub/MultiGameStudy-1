using Rogue.Ingame.Vfx;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [CreateAssetMenu(fileName = "CommonResources", menuName = "Data/Common Resources")]
    public class CommonResources : ScriptableObject
    {
        [Required] public VfxObject StunVfx;
        [Required] public GameObject MyCharacterIndicator;
        [Required] public MaterialControlData HitFlashMaterialData;

        private static CommonResources instance;
        public static CommonResources Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<CommonResources>("CommonResources");
                return instance;
            }
        }
    }
}