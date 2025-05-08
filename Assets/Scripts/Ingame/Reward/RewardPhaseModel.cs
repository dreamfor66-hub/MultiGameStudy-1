using System;
using Rogue.Ingame.Stage;

namespace Rogue.Ingame.Reward
{
    public enum RewardPhase
    {
        None,
        SelectReward,
        SelectRemove,
    }

    public class RewardPhaseModel
    {
        public Action OnChanged;
        public RewardPhase Phase { get; private set; }
        public RewardLevel SelectedReward { get; private set; }

        public RewardPhaseModel()
        {
            Phase = RewardPhase.None;
        }

        private void SetPhase(RewardPhase phase)
        {
            if (Phase != phase)
            {
                Phase = phase;
                OnChanged?.Invoke();
            }
        }

        public void GoRemovePhase(RewardLevel selected)
        {
            SelectedReward = selected;
            SetPhase(RewardPhase.SelectRemove);
        }

        public void GoSelectPhase()
        {
            SetPhase(RewardPhase.SelectReward);
        }

        public void Clear()
        {
            SetPhase(RewardPhase.None);
        }
    }
}