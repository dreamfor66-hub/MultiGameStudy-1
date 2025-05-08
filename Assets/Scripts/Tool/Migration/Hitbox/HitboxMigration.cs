using System;
using System.Linq;
using Rogue.Ingame.Data;
using Rogue.Tool.Timeline.Playables.HitCollider;
using UnityEditor;
using UnityEngine.Timeline;

namespace Rogue.Tool.Migration.Hitbox
{
    public static class HitboxMigration
    {
        public static void Migrate(TimelineAsset asset, Action<HitboxInfo> migrateAction)
        {
            var tracks = asset.GetOutputTracks().OfType<HitColliderTrack>();
            foreach (var track in tracks)
            {
                foreach (var clip in track.GetClips())
                {
                    var colliderClip = clip.asset as HitColliderClip;
                    migrateAction(colliderClip.Info);
                }
            }
            EditorUtility.SetDirty(asset);
        }

        public static void Migrate(ActionData actionData, Action<HitboxInfo> migrateAction)
        {
            foreach (var hitbox in actionData.AttackHitboxData)
            {
                migrateAction(hitbox.Info);
            }
            EditorUtility.SetDirty(actionData);
        }

        public static void Migrate(BulletData bulletData, Action<HitboxInfo> migrateAction)
        {
            foreach (var hitbox in bulletData.Attack.Attacks)
            {
                migrateAction(hitbox.HitboxInfo);
            }
            EditorUtility.SetDirty(bulletData);
        }
    }
}
