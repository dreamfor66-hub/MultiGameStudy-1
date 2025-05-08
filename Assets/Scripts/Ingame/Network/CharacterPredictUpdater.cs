using System;
using Rogue.Ingame.Character;
using Rogue.Ingame.Input;

namespace Rogue.Ingame.Network
{
    public class CharacterPredictUpdater
    {
        private readonly CharacterBehaviour playerCharacter;
        private readonly Action<int> onBeforeUpdate;
        private readonly InputConverter inputConverter;
        public int Frame { get; private set; }


        public CharacterPredictUpdater(CharacterBehaviour character, Action<int> onBeforeUpdate)
        {
            playerCharacter = character;
            this.onBeforeUpdate = onBeforeUpdate;
            Frame = 0;
            inputConverter = new InputConverter();
        }

        public void Reset(StateFrame stateFrame, InputState input)
        {
            playerCharacter.SyncOnly(stateFrame.TotalInfo, stateFrame.ServerFrame);
            Frame = stateFrame.ServerFrame;
            inputConverter.Convert(input);
        }

        public void Update(InputState input)
        {
            Frame++;
            onBeforeUpdate(Frame);
            var control = inputConverter.Convert(input);
            playerCharacter.PredictUpdateFrame(control);
        }

        public void UpdateUntil(int frame, InputState input)
        {
            while (Frame < frame)
                Update(input);
        }

        public StateFrame GetStateFrame()
        {
            return new StateFrame(playerCharacter.TotalInfo, Frame);
        }

        public override string ToString()
        {
            var stateInfo = playerCharacter.Character.StateInfo;
            return $"[{Frame}] : {stateInfo.StateType}-{stateInfo.Frame}";
        }
    }
}