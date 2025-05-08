using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rogue.Ingame.Stage
{
    [RequireComponent(typeof(Rigidbody))]
    public class TriggerArea : MonoBehaviour
    {
        public UnityEvent OnAction;

        public void OnTriggerEnter(Collider other)
        {
            OnAction.Invoke();
        }
    }
}