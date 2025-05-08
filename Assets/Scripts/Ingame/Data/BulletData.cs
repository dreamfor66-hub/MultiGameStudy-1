using System;
using System.Collections.Generic;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    public enum BulletEvent
    {
        Init,
        Hit,
        Hurt,
        Despawn,
        CollideMap,
        DpDead,
        HitOnlyBody,
    }

    [Serializable]
    [HideLabel]
    public class BulletCommonData
    {
        public int LifeTimeFrame = 300;
        public bool IsTeamFixed;
        [ShowIf(nameof(IsTeamFixed))]
        public Team FixedTeam;
        public List<BulletEvent> DespawnBy;
        public int DespawnDelayFrame;
    }

    [CreateAssetMenu(fileName = "new BulletData", menuName = "Data/Bullet")]
    public class BulletData : ScriptableObject
    {
        public BulletCommonData Common = new BulletCommonData();
        public BulletAttackData Attack = new BulletAttackData();
        public BulletVfxData Vfx = new BulletVfxData();
        public BulletSpawnBulletData SpawnBullet = new BulletSpawnBulletData();
        public BulletMoveParabolaData MoveParabola = new BulletMoveParabolaData();
        public DpData Dp = new DpData();
    }
}