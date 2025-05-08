using Rogue.Ingame.Camera;
using Rogue.Ingame.UI;
using UnityEngine;

namespace Rogue.Ingame.Character.StatusImpl
{
    public class CharacterStatusInvisible : ICharacterStatusImpl
    {
        private readonly Renderer[] renderers;
        private readonly CameraTarget[] camTargets;
        private readonly HpUI[] hpUis;
        public CharacterStatusInvisible(GameObject gameObject)
        {
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            camTargets = gameObject.GetComponentsInChildren<CameraTarget>();
            hpUis = gameObject.GetComponentsInChildren<HpUI>();
        }

        public void TurnOn()
        {
            foreach (var ren in renderers)
            {
                ren.enabled = false;
            }

            foreach (var camTarget in camTargets)
            {
                camTarget.enabled = false;
            }

            foreach (var ui in hpUis)
            {
                ui.gameObject.SetActive(false);
            }
        }

        public void TurnOff()
        {
            foreach (var ren in renderers)
            {
                ren.enabled = true;
            }
            foreach (var camTarget in camTargets)
            {
                camTarget.enabled = true;
            }
            foreach (var ui in hpUis)
            {
                ui.gameObject.SetActive(true);
            }
        }
    }
}