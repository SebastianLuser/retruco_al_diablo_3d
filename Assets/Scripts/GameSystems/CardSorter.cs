using System.Collections.Generic;
using Components.Cards;
using GameSystems;

public static class CardSorter
{
    public static void QuickSort(List<Card> cards, SortType sortType)
    {
        if (cards == null || cards.Count <= 1)
            return;
            
        QuickSortRecursive(cards, 0, cards.Count - 1, sortType);
    }
    
    private static void QuickSortRecursive(List<Card> cards, int low, int high, SortType sortType)
    {
        if (low < high)
        {
            int pivotIndex = Partition(cards, low, high, sortType);
            
            QuickSortRecursive(cards, low, pivotIndex - 1, sortType);
            QuickSortRecursive(cards, pivotIndex + 1, high, sortType);
        }
    }
    
    private static int Partition(List<Card> cards, int low, int high, SortType sortType)
    {
        Card pivot = cards[high];
        int i = low - 1;
        
        for (int j = low; j < high; j++)
        {
            if (CompareCards(cards[j], pivot, sortType) <= 0)
            {
                i++;
                Swap(cards, i, j);
            }
        }
        
        Swap(cards, i + 1, high);
        return i + 1;
    }
    
    private static void Swap(List<Card> cards, int i, int j)
    {
        Card temp = cards[i];
        cards[i] = cards[j];
        cards[j] = temp;
    }
    
    private static int CompareCards(Card a, Card b, SortType sortType)
    {
        switch (sortType)
        {
            case SortType.BySuit:
                int suitComparison = ((int)a.suit).CompareTo((int)b.suit);
                if (suitComparison != 0)
                    return suitComparison;
                return ((int)a.rank).CompareTo((int)b.rank);
                
            case SortType.ByPower:
                int powerComparison = a.GetPower().CompareTo(b.GetPower());
                if (powerComparison != 0)
                    return powerComparison;
                return ((int)a.suit).CompareTo((int)b.suit);
                
            default:
                return 0;
        }
    }
}