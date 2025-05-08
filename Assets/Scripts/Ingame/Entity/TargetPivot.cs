using System;
using System.Collections.Generic;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Character;
using UnityEngine;

namespace Rogue.Ingame.Entity
{
    public class TargetPivot : MonoBehaviour
    {
        public static List<TargetPivot> Pivots = new List<TargetPivot>();
        public IEntity Entity { get; private set; }

        void Awake()
        {
            var character = GetComponent<CharacterBehaviour>();
            if (character != null)
                Entity = character;

            var bullet = GetComponent<BulletBehaviour>();
            if (bullet != null)
                Entity = bullet;
        }

        void OnEnable()
        {
            Pivots.Add(this);
        }

        void OnDisable()
        {
            Pivots.Remove(this);
        }
    }
}
