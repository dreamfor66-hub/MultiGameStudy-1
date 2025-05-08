using System.Runtime.Remoting.Messaging;
using Rogue.Ingame.Attack;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class HitboxGizmo : MonoBehaviour
    {
        [SerializeField] private CharacterBehaviour character;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var actionData = character.CharacterAttack.ActionData;
            if (actionData == null)
                return;
            var frame = character.CharacterAttack.Frame;


            foreach (var hitbox in actionData.AttackHitboxData)
            {
                if (frame < hitbox.StartFrame || hitbox.EndFrame < frame)
                    continue;
                var anchor = character.CharacterAttack.GetAnchor(hitbox.ColliderId);
                if (anchor == null)
                    continue;

                foreach (var collider in anchor.Colliders)
                {
                    if (collider is ContinuousSphereCollider sc)
                    {
                        var scale = sc.transform.lossyScale;
                        var radius = Mathf.Max(scale.x, scale.y, scale.z) * sc.Radius;
                        Gizmos.DrawSphere(sc.transform.position, radius);
                    }
                }
            }
        }
    }
}