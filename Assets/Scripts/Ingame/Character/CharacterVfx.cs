using System.Collections.Generic;
using Rogue.Ingame.Data;
using Rogue.Ingame.Vfx;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class CharacterVfx
    {
        private readonly Transform transform;

        private readonly HashSet<int> spawned = new HashSet<int>();
        private readonly List<(VfxObject vfx, int startFrame)> manualVfxs = new List<(VfxObject vfx, int startFrame)>();

        public CharacterVfx(Transform transform)
        {
            this.transform = transform;
        }

        public void OnDestroy()
        {
            ClearState();
        }

        private void ClearState()
        {
            foreach (var (vfx, _) in manualVfxs)
            {
                vfx.DespawnManually();
            }

            spawned.Clear();
            manualVfxs.Clear();
        }

        public void UpdateVfx(CharacterStateUpdateInfo update)
        {
            if (update.Cur.Id != update.Prev.Id)
            {
                ClearState();
            }


            if (update.Cur.StateType == CharacterStateType.Action)
            {
                var actionData = update.Cur.ActionData;
                var frame = update.Cur.Frame;

                for (var i = 0; i < actionData.VfxData.Count; i++)
                {
                    if (spawned.Contains(i))
                        continue;
                    var data = actionData.VfxData[i];
                    if (data.StartFrame <= frame)
                    {
                        if (data.Prefab != null)
                        {
                            var obj = VfxSpawner.Spawn(data.Prefab, transform, data.Position, data.Rotation, data.Scale);
                            if (obj.VfxType == VfxType.Manual)
                                manualVfxs.Add((obj, data.StartFrame));
                        }
                        spawned.Add(i);
                    }
                }

                foreach (var (vfx, startFrame) in manualVfxs)
                {
                    var time = (frame - startFrame) / CommonVariables.GameFrame;
                    vfx.SetTime(time);
                }

            }

            UpdateStun(update.Cur.StunFrame > 0 && update.Cur.StateType != CharacterStateType.Dead);
        }

        private VfxObject stunVfx = null;
        private void UpdateStun(bool isStun)
        {
            if (isStun && stunVfx == null)
            {
                //TODO : height 적용
                stunVfx = VfxSpawner.Spawn(CommonResources.Instance.StunVfx, transform, new Vector3(0f, 3.5f, 0f),
                    Quaternion.Euler(90, 0, 0), Vector3.one);
            }
            else if (!isStun && stunVfx != null)
            {
                stunVfx.DespawnManually();
                stunVfx = null;
            }
        }
    }
}