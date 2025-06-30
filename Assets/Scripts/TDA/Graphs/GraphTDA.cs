using System;
using System.Collections.Generic;
using TDA.Sets;

namespace TDA.Graphs
{
    public class Graph<T> : IGraph<T>
    {
        private int[,] adjacencyMatrix;
        private T[] vertices;
        private int vertexCount;
        private int capacity;
        private IComparer<T> comparer;

        public Graph(int capacity = 100)
        {
            this.capacity = capacity;
            adjacencyMatrix = new int[capacity, capacity];
            vertices = new T[capacity];
            vertexCount = 0;
            comparer = Comparer<T>.Default;
        }

        public Graph(int capacity, IComparer<T> customComparer) : this(capacity)
        {
            comparer = customComparer;
        }

        public void SetComparer(IComparer<T> customComparer)
        {
            comparer = customComparer;
        }

        public void AddVertex(T vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (vertexCount >= capacity)
                throw new InvalidOperationException("Graph is full");

            if (GetVertexIndex(vertex) != -1)
                return;

            vertices[vertexCount] = vertex;

            for (int i = 0; i <= vertexCount; i++)
            {
                adjacencyMatrix[vertexCount, i] = 0;
                adjacencyMatrix[i, vertexCount] = 0;
            }

            vertexCount++;
        }

        public bool RemoveVertex(T vertex)
        {
            if (vertex == null)
                return false;

            int index = GetVertexIndex(vertex);
            if (index == -1)
                return false;

            for (int k = 0; k < vertexCount; k++)
            {
                adjacencyMatrix[k, index] = adjacencyMatrix[k, vertexCount - 1];
                adjacencyMatrix[index, k] = adjacencyMatrix[vertexCount - 1, k];
            }

            vertices[index] = vertices[vertexCount - 1];
            vertices[vertexCount - 1] = default(T);

            vertexCount--;
            return true;
        }

        public ISetTDA<T> GetVertices()
        {
            DynamicSet<T> result = new DynamicSet<T>(comparer);
            for (int i = 0; i < vertexCount; i++)
            {
                result.Add(vertices[i]);
            }

            return result;
        }

        public void AddEdge(T source, T destination, int weight = 1)
        {
            if (source == null || destination == null)
                throw new ArgumentNullException();

            if (weight < 0)
                throw new ArgumentException("Weight cannot be negative");

            int sourceIndex = GetVertexIndex(source);
            int destIndex = GetVertexIndex(destination);

            if (sourceIndex == -1 || destIndex == -1)
                throw new ArgumentException("Vertex not found in graph");

            adjacencyMatrix[sourceIndex, destIndex] = weight;
        }

        public bool RemoveEdge(T source, T destination)
        {
            if (source == null || destination == null)
                return false;

            int sourceIndex = GetVertexIndex(source);
            int destIndex = GetVertexIndex(destination);

            if (sourceIndex == -1 || destIndex == -1)
                return false;

            adjacencyMatrix[sourceIndex, destIndex] = 0;
            return true;
        }

        public bool HasEdge(T source, T destination)
        {
            if (source == null || destination == null)
                return false;

            int sourceIndex = GetVertexIndex(source);
            int destIndex = GetVertexIndex(destination);

            if (sourceIndex == -1 || destIndex == -1)
                return false;

            return adjacencyMatrix[sourceIndex, destIndex] != 0;
        }

        public int GetEdgeWeight(T source, T destination)
        {
            if (!HasEdge(source, destination))
                throw new ArgumentException("Edge does not exist");

            int sourceIndex = GetVertexIndex(source);
            int destIndex = GetVertexIndex(destination);

            return adjacencyMatrix[sourceIndex, destIndex];
        }

        public bool IsEmpty()
        {
            return vertexCount == 0;
        }

        public int VertexCount()
        {
            return vertexCount;
        }

        public int EdgeCount()
        {
            int count = 0;
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (adjacencyMatrix[i, j] != 0)
                        count++;
                }
            }

            return count;
        }

        public void Clear()
        {
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = default(T);
                for (int j = 0; j < vertexCount; j++)
                {
                    adjacencyMatrix[i, j] = 0;
                }
            }

            vertexCount = 0;
        }

        public List<T> GetPath(T source, T destination)
        {
            if (source == null || destination == null)
                throw new ArgumentNullException();

            return BreadthFirstSearchPath(source, destination);
        }

        public List<T> DepthFirstSearch(T startVertex)
        {
            if (startVertex == null)
                throw new ArgumentNullException(nameof(startVertex));

            int startIndex = GetVertexIndex(startVertex);
            if (startIndex == -1)
                throw new ArgumentException("Start vertex not found");

            List<T> result = new List<T>();
            bool[] visited = new bool[vertexCount];

            DFSUtil(startIndex, visited, result);
            return result;
        }

        public List<T> BreadthFirstSearch(T startVertex)
        {
            if (startVertex == null)
                throw new ArgumentNullException(nameof(startVertex));

            int startIndex = GetVertexIndex(startVertex);
            if (startIndex == -1)
                throw new ArgumentException("Start vertex not found");

            List<T> result = new List<T>();
            bool[] visited = new bool[vertexCount];
            Queue<int> queue = new Queue<int>();

            visited[startIndex] = true;
            queue.Enqueue(startIndex);

            while (queue.Count > 0)
            {
                int currentIndex = queue.Dequeue();
                result.Add(vertices[currentIndex]);

                for (int j = 0; j < vertexCount; j++)
                {
                    if (adjacencyMatrix[currentIndex, j] != 0 && !visited[j])
                    {
                        visited[j] = true;
                        queue.Enqueue(j);
                    }
                }
            }

            return result;
        }

        public void DisplayGraph()
        {
            if (vertexCount == 0)
            {
                Console.WriteLine("Graph is empty");
                return;
            }

            Console.WriteLine("Graph Vertices:");
            for (int i = 0; i < vertexCount; i++)
            {
                Console.WriteLine($"  {i}: {vertices[i]}");
            }

            Console.WriteLine("\nGraph Edges (Source → Destination, Weight):");
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (adjacencyMatrix[i, j] != 0)
                    {
                        Console.WriteLine($"  {vertices[i]} → {vertices[j]}, {adjacencyMatrix[i, j]}");
                    }
                }
            }

            Console.WriteLine("\nAdjacency Matrix:");
            Console.Write("     ");
            for (int i = 0; i < vertexCount; i++)
            {
                Console.Write($"{vertices[i],4}");
            }

            Console.WriteLine();

            for (int i = 0; i < vertexCount; i++)
            {
                Console.Write($"{vertices[i],4} ");
                for (int j = 0; j < vertexCount; j++)
                {
                    Console.Write($"{adjacencyMatrix[i, j],4}");
                }

                Console.WriteLine();
            }
        }

        private int GetVertexIndex(T vertex)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                if (comparer.Compare(vertices[i], vertex) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private void DFSUtil(int vertexIndex, bool[] visited, List<T> result)
        {
            visited[vertexIndex] = true;
            result.Add(vertices[vertexIndex]);

            for (int j = 0; j < vertexCount; j++)
            {
                if (adjacencyMatrix[vertexIndex, j] != 0 && !visited[j])
                {
                    DFSUtil(j, visited, result);
                }
            }
        }

        private List<T> BreadthFirstSearchPath(T source, T destination)
        {
            int sourceIndex = GetVertexIndex(source);
            int destIndex = GetVertexIndex(destination);

            if (sourceIndex == -1 || destIndex == -1)
                return new List<T>();

            bool[] visited = new bool[vertexCount];
            int[] parent = new int[vertexCount];
            Queue<int> queue = new Queue<int>();

            for (int i = 0; i < vertexCount; i++)
                parent[i] = -1;

            visited[sourceIndex] = true;
            queue.Enqueue(sourceIndex);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                if (current == destIndex)
                {
                    List<T> path = new List<T>();
                    int pathIndex = destIndex;

                    while (pathIndex != -1)
                    {
                        path.Insert(0, vertices[pathIndex]);
                        pathIndex = parent[pathIndex];
                    }

                    return path;
                }

                for (int j = 0; j < vertexCount; j++)
                {
                    if (adjacencyMatrix[current, j] != 0 && !visited[j])
                    {
                        visited[j] = true;
                        parent[j] = current;
                        queue.Enqueue(j);
                    }
                }
            }

            return new List<T>();
        }
    }
}