using System.Collections.Generic;

namespace TDA.Hierarchical
{


    public interface ITree<T>
    {
        void Insert(T element);
        bool Search(T element);
        bool Remove(T element);
        bool IsEmpty();
        int CountNodes();

        int Height();

        T FindMinimum();
        T FindMaximum();
        T FindSuccessor(T element);
        T FindPredecessor(T element);

        void InOrder();
        void PreOrder();
        void PostOrder();
        void LevelOrder();

        bool IsValidBST();
        List<T> GetElementsInRange(T min, T max);
        List<T> GetSortedElements();
        List<T> GetSortedElementsDescending();
        
        void SetComparer(IComparer<T> comparer);

        void DisplayTree();
        bool IsBalanced();
    }
}