using System;
using System.Collections.Generic;

namespace TDA.Sets
{ 
    public class SetNode<T>
    {
        public T Data { get; set; }
        public SetNode<T> Next { get; set; }

        public SetNode(T data)
        {
            Data = data;
            Next = null;
        }
    }

    public class DynamicSet<T> : ISetTDA<T>
    {
        private SetNode<T> head;
        private int count;
        private IComparer<T> comparer;

        public DynamicSet()
        {
            head = null;
            count = 0;
            comparer = Comparer<T>.Default;
        }

        public DynamicSet(IComparer<T> customComparer)
        {
            head = null;
            count = 0;
            comparer = customComparer;
        }

        public void SetComparer(IComparer<T> customComparer)
        {
            comparer = customComparer;
        }

        public void Add(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!Contains(element))
            {
                SetNode<T> newNode = new SetNode<T>(element);
                newNode.Next = head;
                head = newNode;
                count++;
            }
        }

        public bool Remove(T element)
        {
            if (element == null || head == null)
                return false;

            if (comparer.Compare(head.Data, element) == 0)
            {
                head = head.Next;
                count--;
                return true;
            }

            SetNode<T> current = head;
            while (current.Next != null && comparer.Compare(current.Next.Data, element) != 0)
            {
                current = current.Next;
            }

            if (current.Next != null)
            {
                current.Next = current.Next.Next;
                count--;
                return true;
            }

            return false;
        }

        public bool Contains(T element)
        {
            if (element == null)
                return false;

            SetNode<T> current = head;
            while (current != null && comparer.Compare(current.Data, element) != 0)
            {
                current = current.Next;
            }
            return current != null;
        }

        public bool IsEmpty()
        {
            return head == null;
        }

        public int Count()
        {
            return count;
        }

        public T GetAny()
        {
            if (head == null)
                throw new InvalidOperationException("Set is empty");
            
            return head.Data;
        }

        public void Clear()
        {
            head = null;
            count = 0;
        }

        public List<T> ToList()
        {
            List<T> result = new List<T>();
            SetNode<T> current = head;
            
            while (current != null)
            {
                result.Add(current.Data);
                current = current.Next;
            }
            
            return result;
        }

        public ISetTDA<T> Union(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            DynamicSet<T> result = new DynamicSet<T>(comparer);
            
            foreach (T element in this.ToList())
            {
                result.Add(element);
            }
            
            foreach (T element in other.ToList())
            {
                result.Add(element);
            }
            
            return result;
        }

        public ISetTDA<T> Intersection(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            DynamicSet<T> result = new DynamicSet<T>(comparer);
            
            foreach (T element in this.ToList())
            {
                if (other.Contains(element))
                {
                    result.Add(element);
                }
            }
            
            return result;
        }

        public ISetTDA<T> Difference(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            DynamicSet<T> result = new DynamicSet<T>(comparer);
            
            foreach (T element in this.ToList())
            {
                if (!other.Contains(element))
                {
                    result.Add(element);
                }
            }
            
            return result;
        }

        public bool IsSubsetOf(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (T element in this.ToList())
            {
                if (!other.Contains(element))
                {
                    return false;
                }
            }
            
            return true;
        }
    }

    public class StaticSet<T> : ISetTDA<T>
    {
        private T[] elements;
        private int count;
        private int capacity;
        private IComparer<T> comparer;

        public StaticSet(int capacity = 100)
        {
            this.capacity = capacity;
            elements = new T[capacity];
            count = 0;
            comparer = Comparer<T>.Default;
        }

        public StaticSet(int capacity, IComparer<T> customComparer) : this(capacity)
        {
            comparer = customComparer;
        }

        public void SetComparer(IComparer<T> customComparer)
        {
            comparer = customComparer;
        }

        public void Add(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (count >= capacity)
                throw new InvalidOperationException("Set is full");

            if (!Contains(element))
            {
                elements[count] = element;
                count++;
            }
        }

        public bool Remove(T element)
        {
            if (element == null)
                return false;

            int index = FindIndex(element);
            if (index >= 0)
            {
                elements[index] = elements[count - 1];
                elements[count - 1] = default(T);
                count--;
                return true;
            }

            return false;
        }

        public bool Contains(T element)
        {
            if (element == null)
                return false;

            return FindIndex(element) >= 0;
        }

        private int FindIndex(T element)
        {
            for (int i = 0; i < count; i++)
            {
                if (comparer.Compare(elements[i], element) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsEmpty()
        {
            return count == 0;
        }

        public int Count()
        {
            return count;
        }

        public T GetAny()
        {
            if (count == 0)
                throw new InvalidOperationException("Set is empty");
            
            return elements[count - 1];
        }

        public void Clear()
        {
            for (int i = 0; i < count; i++)
            {
                elements[i] = default(T);
            }
            count = 0;
        }

        public List<T> ToList()
        {
            List<T> result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                result.Add(elements[i]);
            }
            return result;
        }

        public ISetTDA<T> Union(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            StaticSet<T> result = new StaticSet<T>(capacity + other.Count(), comparer);
            
            foreach (T element in this.ToList())
            {
                result.Add(element);
            }
            
            foreach (T element in other.ToList())
            {
                result.Add(element);
            }
            
            return result;
        }

        public ISetTDA<T> Intersection(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            StaticSet<T> result = new StaticSet<T>(capacity, comparer);
            
            foreach (T element in this.ToList())
            {
                if (other.Contains(element))
                {
                    result.Add(element);
                }
            }
            
            return result;
        }

        public ISetTDA<T> Difference(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            StaticSet<T> result = new StaticSet<T>(capacity, comparer);
            
            foreach (T element in this.ToList())
            {
                if (!other.Contains(element))
                {
                    result.Add(element);
                }
            }
            
            return result;
        }

        public bool IsSubsetOf(ISetTDA<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (T element in this.ToList())
            {
                if (!other.Contains(element))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}