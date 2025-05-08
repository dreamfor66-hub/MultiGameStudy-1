using JetBrains.Annotations;
using UnityEngine;

namespace Rogue.Ingame.Temp
{
    public class HurtToAnimation : MonoBehaviour
    {
        [SerializeField] private Animation anim;
        [UsedImplicitly]
        public void OnHurt()
        {
            anim.Stop();
            anim.Play();
        }
    }
}
