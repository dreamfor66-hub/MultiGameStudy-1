using System.Collections.Generic;
using System.Linq;
using Photon.Bolt;
using Rogue.Ingame.GameCommand;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.BoltAdapter
{
    public class BoltSelectionArea : EntityBehaviour<IAreaState>
    {
        [SerializeField] [Required] private Image image;

        public override void Attached()
        {
            state.AddCallback("Progress", ProgressChanged);
        }

        private void ProgressChanged()
        {
            image.fillAmount = state.Progress / 100f;
        }

        private List<BoltPlayerController> inPlayers = new List<BoltPlayerController>();
        private float progress = 0f;
        private bool isDone = false;

        private void FixedUpdate()
        {
            if (!entity.IsOwner)
                return;
            if (isDone)
                return;

            inPlayers.RemoveAll(x => x == null); // 중간에 나가는 경우에 대한 처리

            var aliveCount = BoltPlayerObjectRegistry.AllPlayers.Count(x => x.Character != null && !x.Character.Character.IsDead);
            var inCount = inPlayers.Count(x => !x.Character.IsDead);

            if (inCount == 0)
                progress = 0f;
            else if (inCount < aliveCount)
                progress += Time.fixedDeltaTime * 100f / 30f;
            else
                progress += Time.fixedDeltaTime * 100f;

            state.Progress = Mathf.Clamp((int)progress, 0, 100);
            if (progress >= 100f)
            {
                GameCommandNextStage.Send();
                BoltNetwork.Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!entity.IsOwner)
                return;

            var player = other.gameObject.GetComponent<BoltPlayerController>();
            if (player != null)
            {
                inPlayers.Add(player);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!entity.IsOwner)
                return;

            var player = other.gameObject.GetComponent<BoltPlayerController>();
            if (player != null)
            {
                inPlayers.Remove(player);
            }
        }
    }
}