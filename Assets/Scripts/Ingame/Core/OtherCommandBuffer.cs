using System.Collections.Generic;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Core
{
    public static class OtherCommandBuffer
    {
        private static readonly List<IGameCommand> buffer = new List<IGameCommand>();

        static OtherCommandBuffer()
        {
            GameCommandStun.Listen(OnStun);
            GameCommandGainStatus.Listen(OnGainSuperArmor);
            GameCommandParryingSuccess.Listen(OnParryingSuccess);
            GameCommandForceAction.Listen(OnForceAction);
            GameCommandRevive.Listen(OnRevive);
            GameCommandAddStack.Listen(OnAddStack);
            GameCommandGainRootAggro.Listen(OnGainAggro);
        }

        public static void GetResult(ref List<IGameCommand> commands)
        {
            commands.Clear();
            commands.AddRange(buffer);
            buffer.Clear();
        }

        private static void OnStun(GameCommandStun cmd)
        {
            buffer.Add(cmd);
        }

        private static void OnGainSuperArmor(GameCommandGainStatus cmd)
        {
            buffer.Add(cmd);
        }

        private static void OnParryingSuccess(GameCommandParryingSuccess cmd)
        {
            buffer.Add(cmd);
        }

        private static void OnForceAction(GameCommandForceAction cmd)
        {
            buffer.Add(cmd);
        }

        private static void OnRevive(GameCommandRevive cmd)
        {
            buffer.Add(cmd);
        }

        private static void OnAddStack(GameCommandAddStack cmd)
        {
            buffer.Add(cmd);
        }

        private static void OnGainAggro(GameCommandGainRootAggro cmd)
        {
            buffer.Add(cmd);
        }
    }
}