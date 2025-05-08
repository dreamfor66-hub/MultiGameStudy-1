using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Rogue.Tool.Migration.Hitbox
{
    public class KnockbackMigrationWindow : OdinEditorWindow
    {
        public List<TimelineAsset> TimelineAssets;
        public List<ActionData> Actions;
        public List<BulletData> Bullets;

        [MenuItem("Tools/Rogue/Migration/Knockback")]
        public static void ShowWindow()
        {
            var window = GetWindow<KnockbackMigrationWindow>();
            window.Show();
        }

        [Button]
        public void FindAllAssets()
        {
            TimelineAssets = FindAssets<TimelineAsset>();
            Actions = FindAssets<ActionData>();
            Bullets = FindAssets<BulletData>();
        }

        private List<T> FindAssets<T>() where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            return guids.Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToList();
        }

        [Button]
        public void Migration()
        {
            TimelineAssets.ForEach(x => HitboxMigration.Migrate(x, Migrate));
            Actions.ForEach(x => HitboxMigration.Migrate(x, Migrate));
            Bullets.ForEach(x => HitboxMigration.Migrate(x, Migrate));
            AssetDatabase.SaveAssets();
        }

        private void Migrate(HitboxInfo hitboxInfo)
        {
            hitboxInfo.Knockback.Strength = GetStrength(hitboxInfo.Knockback);
        }

        private KnockbackStrength GetStrength(KnockbackData knockback)
        {
            // if (knockback.IsJustDamage)
            //    return KnockbackStrength.JustDamage;

            if (knockback.Distance <= 5f)
                return KnockbackStrength.Low;
            else if (knockback.Distance <= 8f)
                return KnockbackStrength.Mid;
            else
                return KnockbackStrength.High;
        }

    }
}