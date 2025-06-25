using System;

namespace TDA
{

    public class DynamicQueueTDA<T> : IDynamicQueueTDA<T>
    {
        private class Node
        {
            public T Data;
            public Node Next;
            
            public Node(T data)
            {
                Data = data;
                Next = null;
            }
        }
        
        private Node head;
        private Node tail;

        public bool Enqueue(T item)
        {
            Node newNode = new Node(item);
            if (IsEmpty())
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail.Next = newNode;
                tail = newNode;
            }
            return true;
        }
        
        public T Dequeue()
        {
            if (IsEmpty()) throw new InvalidOperationException("DynamicQueue is empty");
            var item = head.Data;
            head = head.Next;
            if (head == null)
            {
                tail = null;
            }
            return item;

        }
        
        public bool IsEmpty()
        {
            return head == null;
        }
        
        public T Front()
        {
            if (!IsEmpty())
            {
                return head.Data;
            }

            throw new InvalidOperationException("DynamicQueue is empty");
        }
        
        public void PrintQueue()
        {
            Node current = head;
            while (current != null)
            {
                Console.WriteLine("Element: " + (current.Data != null ? current.Data.ToString() : "null"));
                current = current.Next;
            }
        }
    }
}