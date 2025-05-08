using System;

namespace Rogue.Ingame.Character
{
    public class PreInputHandler<T>
    {

        public T Command { get; private set; }
        public int RemainFrame { get; private set; }

        public static readonly int PreInputFrame = 20;

        public PreInputHandler()
        {
            Command = default;
            RemainFrame = 0;
        }

        public void Reset(T command, int remainFrame)
        {
            Command = command;
            RemainFrame = remainFrame;
        }

        public void Elapse()
        {
            RemainFrame -= 1;
        }

        public bool IsValid()
        {
            return RemainFrame > 0;
        }

        public void Clear()
        {
            RemainFrame = 0;
        }

        public void Push(T command)
        {
            Command = command;
            RemainFrame = PreInputFrame;
        }

        public void Process(Func<T, bool> doCommand)
        {
            if (!IsValid())
                return;

            if (doCommand(Command))
                Clear();
            else
                Elapse();
        }

    }
}
