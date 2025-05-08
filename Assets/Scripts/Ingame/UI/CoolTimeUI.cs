using System.Collections.Generic;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Network;
using Rogue.Ingame.Util.Pool;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class CoolTimeUI : MonoBehaviour
    {
        private UIPool<CoolTimeSingleUI> pool;
        [SerializeField] private CoolTimeSingleUI prefab;
        private List<CoolTimeSingleUI> ui = new List<CoolTimeSingleUI>();
        private List<ActionData> actions = new List<ActionData>();

        void Awake()
        {
            pool = new UIPool<CoolTimeSingleUI>(prefab);
        }

        void Start()
        {
            OwnerCharacterHolder.OnChanged += OnOwnerCharacterChanged;
            OnOwnerCharacterChanged();
        }

        private CharacterBehaviour character;
        private void OnOwnerCharacterChanged()
        {
            if (OwnerCharacterHolder.OwnerCharacter != null)
            {
                character = OwnerCharacterHolder.OwnerCharacter;
            }
            else
            {
                character = null;
                pool.Clear();
            }
        }
        void LateUpdate()
        {
            if (character == null)
                return;

            var coolTimeModule = character.Character.CoolTimeModule;
            var allActions = character.characterData.PossibleActions;

            foreach (var action in allActions)
            {
                if (action.CoolTimeFrame <= 0)
                    continue;
                var remainFrame = coolTimeModule.RemainFrame(action, ServerFrameHolder.Frame);
                var find = actions.IndexOf(action);
                if (remainFrame <= 0)
                {
                    if (find >= 0)
                    {
                        pool.Return(ui[find]);
                        actions.RemoveAt(find);
                        ui.RemoveAt(find);
                    }
                }
                else
                {
                    if (find >= 0)
                    {
                        ui[find].SetCoolTime(remainFrame, action.CoolTimeFrame);
                    }
                    else
                    {
                        var newUI = pool.Get();
                        ui.Add(newUI);
                        actions.Add(action);
                        newUI.SetCoolTime(remainFrame, action.CoolTimeFrame);
                    }
                }
            }
        }

    }
}
