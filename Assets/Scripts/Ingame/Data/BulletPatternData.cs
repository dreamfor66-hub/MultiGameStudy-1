using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.ExpressionParser;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [Serializable]
    public class VariableData
    {
        public string Key;
        public int Min;
        public int Max;
        public int[] Excepts;
    }

    [Serializable]
    public class BulletPatternGenerateData
    {
        [TableList]
        public List<VariableData> Variables;
        public string Time;
        public string Angle;
        public string Speed;
        public string PosX;
        public string PosY;
        public string PosZ;

        public List<BulletPatternSpawnData> Generate()
        {
            var bullets = new List<BulletPatternSpawnData>();

            var timeCalculator = new ExpressionCalculator(Time);
            var angleCalculator = new ExpressionCalculator(Angle);
            var speedCalculator = new ExpressionCalculator(Speed);
            var posXCalculator = new ExpressionCalculator(PosX);
            var posYCalculator = new ExpressionCalculator(PosY);
            var posZCalculator = new ExpressionCalculator(PosZ);

            VariableForeach((map) =>
            {
                var time = timeCalculator.Calculate(map);
                var angle = angleCalculator.Calculate(map);
                var speed = speedCalculator.Calculate(map);
                var posX = posXCalculator.Calculate(map);
                var posY = posYCalculator.Calculate(map);
                var posZ = posZCalculator.Calculate(map);
                bullets.Add(new BulletPatternSpawnData
                {
                    Time = time,
                    Angle = angle,
                    Speed = speed,
                    Position = new Vector3(posX, posY, posZ)
                });
            });

            bullets.Sort((a, b) => (int)((a.Time - b.Time) * 100000));
            return bullets;
        }


        private void VariableForeach(Action<List<VariableMap>> action)
        {
            var input = new List<VariableMap>();
            VariableForeach(0, action, ref input);
        }

        private void VariableForeach(int idx, Action<List<VariableMap>> action, ref List<VariableMap> input)
        {
            if (idx < Variables.Count)
            {
                var cur = Variables[idx];
                if (cur.Min > cur.Max)
                    throw new ArgumentException("Variable min max error");
                var map = input.Find(x => x.Key == cur.Key);
                if (map == null)
                {
                    map = new VariableMap(cur.Key, 0);
                    input.Add(map);
                }

                for (var i = cur.Min; i <= cur.Max; i++)
                {
                    if (cur.Excepts.Contains(i))
                        continue;
                    map.Value = i;
                    VariableForeach(idx + 1, action, ref input);
                }
            }
            else
            {
                action.Invoke(input);
            }
        }
    }

    [Serializable]
    public class BulletPatternSpawnData
    {
        public float Time;
        public float Angle;
        public float AngleY;
        public float Speed;
        public Vector3 Position;
    }

    [CreateAssetMenu(fileName = "new BulletPatternData", menuName = "Data/BulletPattern")]
    public class BulletPatternData : ScriptableObject
    {
        [Title("Expression Generator")]
        [HideLabel]
        [PropertyOrder(0)]
        public BulletPatternGenerateData GenData;

#if UNITY_EDITOR
        [Button]
        [PropertyOrder(0)]
        public void Generate()
        {
            Bullets = GenData.Generate();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif

        [Title("Bullets")]
        [TableList]
        [PropertyOrder(1)]
        public List<BulletPatternSpawnData> Bullets;
    }
}