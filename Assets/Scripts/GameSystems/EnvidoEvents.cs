using System;

namespace GameSystems
{
    public static class EnvidoEvents
    {
        public static event Action OnCleanupRequested;

        public static void RequestCleanup()
        {
            OnCleanupRequested?.Invoke();
        }
    }
}