using System;
using System.Collections.Generic;

namespace TDA.Hierarchical
{
    public class BSTNode<T>
    {
        public T Data { get; set; }
        public BSTNode<T> Left { get; set; }
        public BSTNode<T> Right { get; set; }
        public BSTNode<T> Parent { get; set; }

        public BSTNode(T data)
        {
            Data = data;
            Left = null;
            Right = null;
            Parent = null;
        }

        public bool IsLeaf()
        {
            return Left == null && Right == null;
        }

        public bool HasBothChildren()
        {
            return Left != null && Right != null;
        }

        public bool HasOnlyLeftChild()
        {
            return Left != null && Right == null;
        }

        public bool HasOnlyRightChild()
        {
            return Left == null && Right != null;
        }
    }
    public class BinarySearchTree<T> : ITree<T>
    {
        public BSTNode<T> Root { get; set; }
        private IComparer<T> comparer;

        public BinarySearchTree()
        {
            Root = null;
            comparer = Comparer<T>.Default;
        }

        public BinarySearchTree(IComparer<T> customComparer)
        {
            Root = null;
            comparer = customComparer;
        }

        public BinarySearchTree(T rootData)
        {
            Root = new BSTNode<T>(rootData);
            comparer = Comparer<T>.Default;
        }

        public BinarySearchTree(T rootData, IComparer<T> customComparer)
        {
            Root = new BSTNode<T>(rootData);
            comparer = customComparer;
        }

        public List<T> GetSortedElementsDescending()
        {
            List<T> result = new List<T>();
            InOrderReverseToList(Root, result);
            return result;
        }
        
        private void InOrderReverseToList(BSTNode<T> node, List<T> result)
        {
            if (node != null)
            {
                InOrderReverseToList(node.Right, result);
                result.Add(node.Data);
                InOrderReverseToList(node.Left, result);
            }
        }

        public void SetComparer(IComparer<T> customComparer)
        {
            comparer = customComparer;
        }

        public void Insert(T element)
        {
            Root = InsertRecursive(Root, element);
        }

        public bool Search(T element)
        {
            return SearchRecursive(Root, element);
        }

        public bool Remove(T element)
        {
            int initialCount = CountNodes();
            Root = RemoveRecursive(Root, element);
            return CountNodes() < initialCount;
        }

        public bool IsEmpty()
        {
            return Root == null;
        }

        public int CountNodes()
        {
            return CountNodesRecursive(Root);
        }

        public int Height()
        {
            return HeightRecursive(Root);
        }

        public T FindMinimum()
        {
            if (Root == null)
                throw new InvalidOperationException("Tree is empty");
            
            BSTNode<T> minNode = FindMinimumNode(Root);
            return minNode.Data;
        }

        public T FindMaximum()
        {
            if (Root == null)
                throw new InvalidOperationException("Tree is empty");
            
            BSTNode<T> maxNode = FindMaximumNode(Root);
            return maxNode.Data;
        }

        public T FindSuccessor(T element)
        {
            BSTNode<T> node = SearchNodeRecursive(Root, element);
            if (node == null)
                throw new ArgumentException("Element not found in tree");

            BSTNode<T> successor = FindSuccessorNode(node);
            if (successor == null)
                throw new ArgumentException("No successor found");

            return successor.Data;
        }

        public T FindPredecessor(T element)
        {
            BSTNode<T> node = SearchNodeRecursive(Root, element);
            if (node == null)
                throw new ArgumentException("Element not found in tree");

            BSTNode<T> predecessor = FindPredecessorNode(node);
            if (predecessor == null)
                throw new ArgumentException("No predecessor found");

            return predecessor.Data;
        }

        public void InOrder()
        {
            if (Root == null)
            {
                Console.WriteLine("Tree is empty");
                return;
            }

            Console.Write("InOrder: ");
            InOrderRecursive(Root);
            Console.WriteLine();
        }

        public void PreOrder()
        {
            if (Root == null)
            {
                Console.WriteLine("Tree is empty");
                return;
            }

            Console.Write("PreOrder: ");
            PreOrderRecursive(Root);
            Console.WriteLine();
        }

        public void PostOrder()
        {
            if (Root == null)
            {
                Console.WriteLine("Tree is empty");
                return;
            }

            Console.Write("PostOrder: ");
            PostOrderRecursive(Root);
            Console.WriteLine();
        }

        public void LevelOrder()
        {
            if (Root == null)
            {
                Console.WriteLine("Tree is empty");
                return;
            }

            Console.Write("LevelOrder: ");
            Queue<BSTNode<T>> queue = new Queue<BSTNode<T>>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                BSTNode<T> current = queue.Dequeue();
                Console.Write(current.Data + " ");

                if (current.Left != null)
                    queue.Enqueue(current.Left);
                if (current.Right != null)
                    queue.Enqueue(current.Right);
            }
            Console.WriteLine();
        }

        public bool IsValidBST()
        {
            return IsValidBSTRecursive(Root, default(T), default(T), false, false);
        }

        public List<T> GetElementsInRange(T min, T max)
        {
            List<T> result = new List<T>();
            GetElementsInRangeRecursive(Root, min, max, result);
            return result;
        }

        public List<T> GetSortedElements()
        {
            List<T> result = new List<T>();
            InOrderToList(Root, result);
            return result;
        }

        private BSTNode<T> InsertRecursive(BSTNode<T> node, T element)
        {
            if (node == null)
                return new BSTNode<T>(element);

            int comparison = comparer.Compare(element, node.Data);

            if (comparison < 0)
            {
                node.Left = InsertRecursive(node.Left, element);
                if (node.Left != null)
                    node.Left.Parent = node;
            }
            else if (comparison > 0)
            {
                node.Right = InsertRecursive(node.Right, element);
                if (node.Right != null)
                    node.Right.Parent = node;
            }

            return node;
        }

        private bool SearchRecursive(BSTNode<T> node, T element)
        {
            if (node == null) return false;

            int comparison = comparer.Compare(element, node.Data);
            
            if (comparison == 0) return true;
            else if (comparison < 0) return SearchRecursive(node.Left, element);
            else return SearchRecursive(node.Right, element);
        }

        private BSTNode<T> SearchNodeRecursive(BSTNode<T> node, T element)
        {
            if (node == null) return null;

            int comparison = comparer.Compare(element, node.Data);
            
            if (comparison == 0) return node;
            else if (comparison < 0) return SearchNodeRecursive(node.Left, element);
            else return SearchNodeRecursive(node.Right, element);
        }

        private BSTNode<T> RemoveRecursive(BSTNode<T> node, T element)
        {
            if (node == null) return null;

            int comparison = comparer.Compare(element, node.Data);

            if (comparison < 0)
            {
                node.Left = RemoveRecursive(node.Left, element);
            }
            else if (comparison > 0)
            {
                node.Right = RemoveRecursive(node.Right, element);
            }
            else
            {
                if (node.IsLeaf())
                    return null;
                
                if (node.HasOnlyLeftChild())
                    return node.Left;
                
                if (node.HasOnlyRightChild())
                    return node.Right;
                
                BSTNode<T> successor = FindMinimumNode(node.Right);
                node.Data = successor.Data;
                node.Right = RemoveRecursive(node.Right, successor.Data);
            }

            return node;
        }

        private BSTNode<T> FindMinimumNode(BSTNode<T> node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        private BSTNode<T> FindMaximumNode(BSTNode<T> node)
        {
            while (node.Right != null)
                node = node.Right;
            return node;
        }

        private BSTNode<T> FindSuccessorNode(BSTNode<T> node)
        {
            if (node.Right != null)
                return FindMinimumNode(node.Right);

            BSTNode<T> current = node;
            BSTNode<T> parent = node.Parent;
            
            while (parent != null && current == parent.Right)
            {
                current = parent;
                parent = parent.Parent;
            }
            
            return parent;
        }

        private BSTNode<T> FindPredecessorNode(BSTNode<T> node)
        {
            if (node.Left != null)
                return FindMaximumNode(node.Left);

            BSTNode<T> current = node;
            BSTNode<T> parent = node.Parent;
            
            while (parent != null && current == parent.Left)
            {
                current = parent;
                parent = parent.Parent;
            }
            
            return parent;
        }

        private int CountNodesRecursive(BSTNode<T> node)
        {
            if (node == null) return 0;
            return 1 + CountNodesRecursive(node.Left) + CountNodesRecursive(node.Right);
        }

        private int HeightRecursive(BSTNode<T> node)
        {
            if (node == null) return -1;
            
            int leftHeight = HeightRecursive(node.Left);
            int rightHeight = HeightRecursive(node.Right);
            
            return Math.Max(leftHeight, rightHeight) + 1;
        }

        private void InOrderRecursive(BSTNode<T> node)
        {
            if (node != null)
            {
                InOrderRecursive(node.Left);
                Console.Write(node.Data + " ");
                InOrderRecursive(node.Right);
            }
        }

        private void PreOrderRecursive(BSTNode<T> node)
        {
            if (node != null)
            {
                Console.Write(node.Data + " ");
                PreOrderRecursive(node.Left);
                PreOrderRecursive(node.Right);
            }
        }

        private void PostOrderRecursive(BSTNode<T> node)
        {
            if (node != null)
            {
                PostOrderRecursive(node.Left);
                PostOrderRecursive(node.Right);
                Console.Write(node.Data + " ");
            }
        }

        private bool IsValidBSTRecursive(BSTNode<T> node, T min, T max, bool hasMin, bool hasMax)
        {
            if (node == null) return true;

            if (hasMin && comparer.Compare(node.Data, min) <= 0) return false;
            if (hasMax && comparer.Compare(node.Data, max) >= 0) return false;

            return IsValidBSTRecursive(node.Left, min, node.Data, hasMin, true) &&
                   IsValidBSTRecursive(node.Right, node.Data, max, true, hasMax);
        }

        private void GetElementsInRangeRecursive(BSTNode<T> node, T min, T max, List<T> result)
        {
            if (node == null) return;

            if (comparer.Compare(node.Data, min) > 0)
                GetElementsInRangeRecursive(node.Left, min, max, result);

            if (comparer.Compare(node.Data, min) >= 0 && comparer.Compare(node.Data, max) <= 0)
                result.Add(node.Data);

            if (comparer.Compare(node.Data, max) < 0)
                GetElementsInRangeRecursive(node.Right, min, max, result);
        }

        private void InOrderToList(BSTNode<T> node, List<T> result)
        {
            if (node != null)
            {
                InOrderToList(node.Left, result);
                result.Add(node.Data);
                InOrderToList(node.Right, result);
            }
        }

        public void DisplayTree()
        {
            if (Root == null)
            {
                Console.WriteLine("Tree is empty");
                return;
            }

            DisplayTreeRecursive(Root, 0, "Root: ");
        }

        private void DisplayTreeRecursive(BSTNode<T> node, int level, string prefix)
        {
            if (node == null) return;

            string indentation = new string(' ', level * 4);
            Console.WriteLine($"{indentation}{prefix}{node.Data}");

            if (node.Left != null || node.Right != null)
            {
                DisplayTreeRecursive(node.Left, level + 1, "L--- ");
                DisplayTreeRecursive(node.Right, level + 1, "R--- ");
            }
        }

        public bool IsBalanced()
        {
            return CheckBalanced(Root) != -1;
        }

        private int CheckBalanced(BSTNode<T> node)
        {
            if (node == null) return 0;

            int leftHeight = CheckBalanced(node.Left);
            if (leftHeight == -1) return -1;

            int rightHeight = CheckBalanced(node.Right);
            if (rightHeight == -1) return -1;

            if (Math.Abs(leftHeight - rightHeight) > 1)
                return -1;

            return Math.Max(leftHeight, rightHeight) + 1;
        }
    }
}