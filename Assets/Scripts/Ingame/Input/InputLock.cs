using System;

namespace Rogue.Ingame.Input
{
    public static class InputLock
    {
        public static bool IsLocked => lockCount > 0;
        private static int lockCount = 0;
        public static IDisposable Lock()
        {
            lockCount++;
            return new InputLockDisposable();
        }

        public static void UnLock()
        {
            lockCount--;
        }
    }
}