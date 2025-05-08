using Rogue.Ingame.Data;
using Rogue.Ingame.EntityMessage;
using Rogue.Ingame.Material;
using UnityEngine;
using Photon.Bolt;

namespace Rogue.Ingame.Character
{
    public class CharacterMaterial
    {
        private readonly CharacterBehaviour character;
        private readonly MaterialControlBehaviour matCon;

        public CharacterMaterial(CharacterBehaviour character)
        {
            this.character = character;
            matCon = character.gameObject.AddComponent<MaterialControlBehaviour>();
            matCon.Renderers = character.GetComponentsInChildren<Renderer>();
        }

        public void OnHurt(HitEvent evnt)
        {
            if (evnt.IsCritical)
            {
                matCon.Run(CommonResources.Instance.HitFlashMaterialData, 0f);
            }
            else if (evnt.IsDirect)
            {
                matCon.Run(CommonResources.Instance.HitFlashMaterialData, 0.5f);
            }
            else
            {
                matCon.Run(CommonResources.Instance.HitFlashMaterialData, 0.9f);
            }
        }
    }
}