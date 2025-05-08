using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rogue
{
    public class AnimatorRandomParameter : StateMachineBehaviour
    {
        [SerializeField] private string parameterName = "Random";
        [SerializeField] private int maxExclusive = 3; // 0부터 n-1까지 설정됨

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (animator != null && maxExclusive > 0)
            {
                int randomValue = Random.Range(0, maxExclusive);
                animator.SetInteger(parameterName, randomValue);
            }
        }

    }
}
