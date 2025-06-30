using System.Collections.Generic;

namespace TDA.Sets
{
    public interface ISetTDA<T>
    {
        void Add(T element);
        bool Remove(T element);
        bool Contains(T element);
        bool IsEmpty();
        int Count();
        T GetAny();
        void Clear();
        void SetComparer(IComparer<T> comparer);
        List<T> ToList();

        ISetTDA<T> Union(ISetTDA<T> other);
        ISetTDA<T> Intersection(ISetTDA<T> other);
        ISetTDA<T> Difference(ISetTDA<T> other);
        bool IsSubsetOf(ISetTDA<T> other);
    }
}