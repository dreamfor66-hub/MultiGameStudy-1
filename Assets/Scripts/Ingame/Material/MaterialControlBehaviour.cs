using System.Collections.Generic;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Material
{
    public class MaterialPlayInfo
    {
        public MaterialControlData Data;
        public float ElapsedTime;
        public float MaxValue;
        public bool IsEnd => Data.DurationType == DurationType.FixedDuration && ElapsedTime > Data.Duration;
    }

    public class MaterialControlBehaviour : MonoBehaviour
    {
        public Renderer[] Renderers;

        private readonly List<MaterialPlayInfo> playings = new List<MaterialPlayInfo>();

        [Button]
        public void Run(MaterialControlData data, float MaxValue = 0f)
        {
            if (data == null)
                return;
            playings.Add(new MaterialPlayInfo
            {
                Data = data,
                ElapsedTime = 0f,
                MaxValue = MaxValue
            });

            playings.Sort((a, b) => b.Data.Priority - a.Data.Priority);
        }

        public void Stop(MaterialControlData data)
        {
            playings.RemoveAll(x => x.Data == data);
        }

        private MaterialPropertyBlock propBlock;

        private void Start()
        {
            propBlock = new MaterialPropertyBlock();
        }

        private void LateUpdate()
        {
            foreach (var info in playings)
            {
                info.ElapsedTime += Time.deltaTime;
            }

            playings.RemoveAll(x => x.IsEnd);

            propBlock.Clear();

            foreach (var info in playings)
            {
                GetProps(info.Data, info.ElapsedTime, info.MaxValue, ref propBlock);
            }

            foreach (var rend in Renderers)
            {
                for (var i = 0; i < rend.materials.Length; i++)
                {
                    rend.SetPropertyBlock(propBlock, i);
                }
            }
        }

        private void GetProps(MaterialControlData data, float time, float MaxValue, ref MaterialPropertyBlock block)
        {
            if (data.Emission.Enabled)
            {
                var value = data.Emission.Curve.Evaluate(time);
                block.SetFloat("_EmissionIntensity", value);
                block.SetColor("_EmissionColor", data.Emission.Color);
            }

            if (data.Fresnel.Enabled)
            {
                var thresholdValue = data.Fresnel.ThresholdCurve.Evaluate(time) + (1 - data.Fresnel.ThresholdCurve.Evaluate(time)) * MaxValue;
                var smoothValue = data.Fresnel.SmoothCurve.Evaluate(time);

                block.SetFloat("_DirectionalFresnel", 0f);
                block.SetColor("_FresnelColor", data.Fresnel.Color);
                block.SetFloat("_FresnelThreshold", thresholdValue);
                block.SetFloat("_FresnelSmooth", smoothValue);
            }
        }
    }
}