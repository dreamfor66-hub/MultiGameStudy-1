using Photon.Bolt;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    public class CreateSelectionArea : MonoBehaviour
    {
        BoltEntity selectionArea = null;

        void Start()
        {
            if (BoltNetwork.IsServer)
            {
                selectionArea = BoltNetwork.Instantiate(BoltPrefabs.PF_Bolt_SelectionArea, this.transform.position,
                    this.transform.rotation);
            }
        }

        private void OnDestroy()
        {
            if (BoltNetwork.IsServer && selectionArea != null)
                BoltNetwork.Destroy(selectionArea);
        }
    }
}