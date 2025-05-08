using System.Collections.Generic;
using Rogue.Ingame.Attack;
using UnityEngine;
using UnityEngine.Playables;

namespace Rogue.Tool.Timeline.Playables.HitCollider
{
    public class HitColliderMixerBehaviour : PlayableBehaviour
    {
        private readonly List<GameObject> objs = new List<GameObject>();

        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ClearObjs();
            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var trackBinding = playerData as HitColliderAnchor;

                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<HitColliderBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                if (inputWeight < 0.5f)
                {
                    continue;
                }

                var customColliders = trackBinding.gameObject.GetComponentsInChildren<AttackCollider>();
                foreach (var collider in customColliders)
                {
                    DrawCollider(collider, input.ShowColor);
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            ClearObjs();
        }


        private void DrawCollider(AttackCollider collider, Color color)
        {
            var obj = CreateObj(collider);
            if (obj == null)
                return;
            PaintColor(obj, color);
            RemoveColliders(obj);
            objs.Add(obj);
        }

        private static GameObject CreateObj(AttackCollider collider)
        {
            switch (collider)
            {
                case ContinuousSphereCollider continuousSphere:
                    return CreateContinuousSphere(continuousSphere);
                default:
                    return null;
            }
        }

        private static GameObject CreateContinuousSphere(ContinuousSphereCollider continuousSphere)
        {
            var target = continuousSphere.transform;
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.transform.position = target.position;
            var scale = Vector3.one * continuousSphere.Radius * 2f;
            scale *= Mathf.Max(target.lossyScale.x, target.lossyScale.y, target.lossyScale.z);
            gameObject.transform.localScale = scale;
            return gameObject;
        }

        private static void PaintColor(GameObject gameObject, Color color)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            Material material = null;
            foreach (var renderer in renderers)
            {
                if (material == null)
                {
                    material = renderer.sharedMaterial != null ? new Material(renderer.sharedMaterial) : new Material(Shader.Find("Standard"));
                    material.color = color;
                }
                renderer.sharedMaterial = material;
            }
        }

        private static void RemoveColliders(GameObject gameObject)
        {
            var colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                Object.DestroyImmediate(collider);
            }
        }

        private void ClearObjs()
        {
            foreach (var obj in objs)
            {
                Object.DestroyImmediate(obj);
            }

            objs.Clear();
        }
    }
}
