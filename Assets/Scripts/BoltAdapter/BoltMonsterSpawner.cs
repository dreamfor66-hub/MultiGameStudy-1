using Photon.Bolt;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class BoltMonsterSpawner : MonoBehaviour
    {
        public void OnGUI()
        {
            // if (GUI.Button(new Rect(0, 0, 300, 100), "Spook"))
            // {
            //     BoltNetwork.Instantiate(BoltPrefabs.PF_Bolt_Monster_Spook, RandomPosition(), Quaternion.identity);
            // }
            // if (GUI.Button(new Rect(0, 100, 300, 100), "Fire"))
            // {
            //     BoltNetwork.Instantiate(BoltPrefabs.PF_Bolt_Monster_Fire, RandomPosition(), Quaternion.identity);
            // }
        }

        private Vector3 RandomPosition()
        {
            return new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        }
    }
}