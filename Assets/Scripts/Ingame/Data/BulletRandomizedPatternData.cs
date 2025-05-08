using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Data
{
    [Serializable]
    public class BulletRandomizedPatternSpawnData
    {
        public int Frame;

        [HideInInspector]
        public float Speed
        {
            get
            {
                if ((initMask & 0b0001) == 0)
                {
                    speed = Random.Range(SpeedRange.x, SpeedRange.y);
                    initMask |= 0b0001;
                }
                return speed;
            }
        }

        [HideInInspector]
        public Vector3 Position
        {
            get
            {
                if ((initMask & 0b0010) == 0)
                {
                    position = new Vector3(PivotPosition.x + Random.Range(PositionMin.x, PositionMax.x),
                                            PivotPosition.y + Random.Range(PositionMin.y, PositionMax.y),
                                            PivotPosition.z + Random.Range(PositionMin.z, PositionMax.z));
                    initMask |= 0b0010;
                }
                return position;
            }
        }
        [HideInInspector]
        public float Angle
        {
            get
            {
                if ((initMask & 0b0100) == 0)
                {
                    angle = Random.Range(AngleRange.x, AngleRange.y);
                    initMask |= 0b0100;
                }
                return angle;
            }
        }

        [HideInInspector]
        public float AngleY
        {
            get
            {
                if ((initMask & 0b1000) == 0)
                {
                    angleY = Random.Range(AngleYRange.x, AngleYRange.y);
                    initMask |= 0b1000;
                }
                return angleY;
            }
        }

        private float speed;
        private Vector3 position;
        private float angle;
        private float angleY;

        public Vector3 PivotPosition;
        public Vector3 PositionMin;
        public Vector3 PositionMax;
        public Vector2 SpeedRange;
        public Vector2 AngleRange;
        public Vector2 AngleYRange;

        private int initMask;
    }

    [CreateAssetMenu(fileName = "new BulletRandomizedPatternData", menuName = "Data/BulletRandomizedPattern")]
    public class BulletRandomizedPatternData : ScriptableObject
    {


        [Title("Bullets")]
        [TableList]
        [PropertyOrder(1)]

        public List<BulletRandomizedPatternSpawnData> Bullets;
    }
}