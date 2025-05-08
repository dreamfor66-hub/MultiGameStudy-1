using System.Collections.Generic;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Character;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Core
{
    public static class BuffCalculator
    {
        private static readonly Queue<IGameCommand> queue = new Queue<IGameCommand>();
        static BuffCalculator()
        {
            GameCommandAddBuff.Listen(OnAddBuff);
            GameCommandBuffAddTime.Listen(OnBuffAddTime);
            GameCommandBuffRelease.Listen(OnBuffRelease);
            GameCommandBuffAddStack.Listen(OnBuffAddStack);
        }

        private static void OnAddBuff(GameCommandAddBuff cmd)
        {
            queue.Enqueue(cmd);
        }

        private static void OnBuffAddTime(GameCommandBuffAddTime cmd)
        {
            queue.Enqueue(cmd);
        }

        private static void OnBuffRelease(GameCommandBuffRelease cmd)
        {
            queue.Enqueue(cmd);
        }

        private static void OnBuffAddStack(GameCommandBuffAddStack cmd)
        {
            queue.Enqueue(cmd);
        }

        public static void CalculateAll()
        {
            while (queue.Count > 0)
            {
                var cmd = queue.Dequeue();

                if (cmd is GameCommandAddBuff cmdAddBuff)
                {
                    if (cmdAddBuff.Target is CharacterBehaviour character)
                    {
                        character.BuffAccepter.AddBuff(cmdAddBuff.BuffData, cmdAddBuff.RootSource);
                        BuffTriggerDispatcher.BuffApply(cmdAddBuff.RootSource, cmdAddBuff.Target, cmdAddBuff.BuffData);
                    }
                }
                else if (cmd is GameCommandBuffAddTime cmdBuffAddTime)
                {
                    if (cmdBuffAddTime.Target is CharacterBehaviour character)
                    {
                        character.BuffAccepter.ChangeBuffTime(cmdBuffAddTime.BuffId, cmdBuffAddTime.TimeModifier, cmdBuffAddTime.IsDurationChanged);
                    }
                }
                else if (cmd is GameCommandBuffRelease cmdBuffRelease)
                {
                    if (cmdBuffRelease.Target is CharacterBehaviour character)
                    {
                        character.BuffAccepter.ReleaseBuff(cmdBuffRelease.BuffId);
                    }
                }
                else if (cmd is GameCommandBuffAddStack cmdBuffAddStack)
                {
                    if (cmdBuffAddStack.Target is CharacterBehaviour character)
                    {
                        character.BuffAccepter.AddBuffStack(cmdBuffAddStack.BuffId, cmdBuffAddStack.Value);
                    }
                }
            }
        }
    }
}