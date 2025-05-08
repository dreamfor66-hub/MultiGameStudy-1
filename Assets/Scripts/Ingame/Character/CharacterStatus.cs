using System.Collections.Generic;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Character.StatusImpl;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class CharacterStatus
    {
        private readonly Dictionary<CharacterStatusType, ICharacterStatusImpl> implements
            = new Dictionary<CharacterStatusType, ICharacterStatusImpl>();

        private readonly List<CharacterStatusType> curStatus = new List<CharacterStatusType>();
        private StatusBuffModule statusBuffModule;

        public CharacterStatus(GameObject obj)
        {
            implements.Add(CharacterStatusType.Ghost, new CharacterStatusGhost(obj));
            implements.Add(CharacterStatusType.Intangible, new CharacterStatusIntangible(obj));
            implements.Add(CharacterStatusType.Invisible, new CharacterStatusInvisible(obj));
            implements.Add(CharacterStatusType.Parrying, new CharacterStatusParrying(obj));
            implements.Add(CharacterStatusType.SuperArmor, new CharacterStatusSuperArmor(obj));
            statusBuffModule = new StatusBuffModule();
        }

        private readonly List<CharacterStatusType> statusActionList = new List<CharacterStatusType>();
        private readonly List<CharacterStatusType> statusBuffList = new List<CharacterStatusType>();
        public void UpdateStatus(CharacterStateUpdateInfo updateInfo)
        {
            if (updateInfo.Cur.StateType == CharacterStateType.Action)
            {
                var frame = Mathf.Clamp(updateInfo.Cur.Frame, 0, updateInfo.Cur.ActionData.TotalFrame);
                UpdateAction(updateInfo.Cur.ActionData, frame);
            }
            else if (updateInfo.Cur.StateType == CharacterStateType.Hurt)
            {
                var knockbackFrame = KnockbackCalculator.TotalFrame(updateInfo.Cur.KnockbackDistance);
                var curFrame = updateInfo.Cur.Frame;
                if (curFrame <= knockbackFrame)
                    KnockbackIntangible();
                else
                    Clear();

            }
            else if (updateInfo.Cur.StateType == CharacterStateType.Dead)
            {
                Die();
            }
            else
            {
                Clear();
            }
        }

        private void UpdateAction(ActionData actionData, float frame)
        {
            statusActionList.Clear();
            foreach (var status in actionData.StatusData)
            {
                if (status.StartFrame <= frame && frame <= status.EndFrame)
                {
                    statusActionList.Add(status.Type);
                }
            }

            ChangeStatus();
        }

        public void UpdateFrame()
        {
            statusBuffList.Clear();
            statusBuffModule.UpdateFrame();
            foreach (var statusBuff in statusBuffModule.StatusBuffs)
            {
                if (statusBuff.Frame > 0 && !statusBuffList.Contains(statusBuff.Type))
                    statusBuffList.Add(statusBuff.Type);
            }
            ChangeStatus();
        }

        public void AddToModule(CharacterStatusType type, int frame)
        {
            statusBuffModule.Add(type, frame);
        }

        private void ChangeStatus()
        {
            var types = new List<CharacterStatusType>();
            types.AddRange(statusActionList);
            types.AddRange(statusBuffList);

            foreach (var cur in curStatus)
            {
                if (!types.Contains(cur))
                    TurnOff(cur);
            }

            foreach (var next in types)
            {
                if (!curStatus.Contains(next))
                    TurnOn(next);
            }

            curStatus.Clear();
            curStatus.AddRange(types);
        }

        private void Clear()
        {
            statusActionList.Clear();
            ChangeStatus();
        }

        private void Die()
        {
            statusActionList.Clear();
            statusActionList.Add(CharacterStatusType.Intangible);
            statusActionList.Add(CharacterStatusType.Ghost);
            ChangeStatus();
        }

        private void KnockbackIntangible()
        {
            statusActionList.Clear();
            statusActionList.Add(CharacterStatusType.Intangible);
            ChangeStatus();
        }

        private void TurnOn(CharacterStatusType type)
        {
            if (implements.ContainsKey(type))
                implements[type].TurnOn();
            else
                Debug.LogError($"not implemented type : {type}");
        }

        private void TurnOff(CharacterStatusType type)
        {
            if (implements.ContainsKey(type))
                implements[type].TurnOff();
            else
                Debug.LogError($"not implemented type : {type}");
        }
    }
}