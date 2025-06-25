using System;

namespace TDA
{
    public class StackTDA<T> : IStackTDA<T>
    {
        private T[] elements;
        private int maxCapacity;
        private int index;
        
        public void InitializeStack(int capacity)
        {
            elements = new T[capacity];
            maxCapacity = capacity;
            index = 0;
        }

        public bool Push(T item)
        {
            if (index >= maxCapacity) return false;
            elements[index] = item;
            index++;
            
            return true;
        }

        public T Pop()
        {
            if (IsEmpty()) throw new InvalidOperationException("StaticStack is empty");
            index--;
            
            return elements[index];

        }

        public T Top()
        {
            if (!IsEmpty())
            {
                return elements[index - 1];
            } 
            
            throw new InvalidOperationException("StaticStack is empty");
        }

        public void PrintStack()
        {
            for (var i = index - 1; i >= 0; i--)
            {
                Console.WriteLine("Element: " + elements[i]);
            }
        }
        
        public bool IsEmpty()
        {
            return index == 0;
        }
    }
}