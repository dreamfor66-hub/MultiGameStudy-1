using FMLib.UI.OnOff;
using Rogue.Ingame.Character;
using Rogue.Ingame.Bullet;
using Sirenix.OdinInspector;
using UnityEngine;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Data;
using System.Threading.Tasks;
using Rogue.Ingame.GameCommand;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

namespace Rogue.Ingame
{
    [Serializable]
    public struct InteractionSequence
    {
        public int FrameToInteract;
        public string Text;
        public List<BuffSpawnBulletData> BulletDataList;
    }

    public class MachineInteractable : InteractableObject
    {
        [SerializeField][Required] private OnOffBehaviour OnOffDead;
        [SerializeField][Required] private BulletBehaviour Parent;
        [SerializeField][Required] private Text Label;
        [SerializeField][Required] private List<InteractionSequence> InteractionSequenceList;
        private int CurSequenceIdx;
        public override int InteractionFrame => InteractionSequenceList[CurSequenceIdx].FrameToInteract;

        private void Awake()
        {
            SetEnable(true);
            SetSelected(false);
            Reset();
        }

        private void Update()
        {
        }

        public override void Interact(CharacterBehaviour interactionCharacter)
        {
            foreach (var bulletData in InteractionSequenceList[CurSequenceIdx].BulletDataList)
                Spawn(bulletData, Parent.transform, Parent.ServerInfo.RootSource);
            if (++CurSequenceIdx >= InteractionSequenceList.Count)
            {
                Reset();
                //To destroy parent
                Parent.Hit();
            }
            Label.text = InteractionSequenceList[CurSequenceIdx].Text;
        }
        private void Reset()
        {
            CurSequenceIdx = 0;
            Label.text = InteractionSequenceList[CurSequenceIdx].Text;
        }

        private async void Spawn(BuffSpawnBulletData data, Transform tm, IEntity rootSource)
        {
            if (data.Frame > 0)
            {
                var delayMs = data.Frame * 1000 / CommonVariables.GameFrame;
                await Task.Delay(delayMs);
            }

            if (tm == null)
                return;

            var position = tm.position;
            var forward = tm.forward;

            var rot = Quaternion.LookRotation(forward, Vector3.up);
            var randomPosition = new Vector3(Random.Range(data.PositionRandomMin.x, data.PositionRandomMax.x),
                Random.Range(data.PositionRandomMin.y, data.PositionRandomMax.y),
                Random.Range(data.PositionRandomMin.z, data.PositionRandomMax.z));
            var pos = position + rot * (data.Position + randomPosition);

            var vel = BulletHelper.GetVelocity(data.Angle, data.AngleY, data.Speed, forward);
            var spawnBullet =
                new GameCommandSpawnBullet(data.BulletPrefab2, pos, vel, rootSource, rootSource.Team, data.ActionType, 0, 0);
            spawnBullet.Send();
        }
    }
}