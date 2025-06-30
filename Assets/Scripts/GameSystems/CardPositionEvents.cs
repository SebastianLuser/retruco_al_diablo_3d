using System;

namespace GameSystems
{
    public enum SortType
    {
        BySuit,
        ByPower
    }

    public static class CardPositionEvents
    {
        public static event Action OnResetPositionsRequested;
        public static event Action<SortType> OnSortRequested;
        public static event Action<SortType> OnDeckSortRequested;

        
        public static void RequestPositionReset()
        {
            OnResetPositionsRequested?.Invoke();
        }
        
        public static void RequestSort(SortType sortType)
        {
            OnSortRequested?.Invoke(sortType);
        }
        public static void RequestDeckSort(SortType sortType)
        {
            OnDeckSortRequested?.Invoke(sortType);
        }
    }
}