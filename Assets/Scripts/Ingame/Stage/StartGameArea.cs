using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    [RequireComponent(typeof(Rigidbody))]
    public class StartGameArea : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
        }
    }
}