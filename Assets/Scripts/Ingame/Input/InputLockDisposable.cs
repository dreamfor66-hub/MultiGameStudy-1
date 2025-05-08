using System;

namespace Rogue.Ingame.Input
{
    public class InputLockDisposable : IDisposable
    {
        public void Dispose()
        {
            InputLock.UnLock();
        }
    }
}