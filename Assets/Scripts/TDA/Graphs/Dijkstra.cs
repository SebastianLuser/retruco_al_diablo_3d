using System;
using System.Collections.Generic;

namespace TDA.Graphs
{
    public class DijkstraResult<T>
    {
        public Dictionary<T, int> Distances { get; set; }
        public Dictionary<T, List<T>> Paths { get; set; }
        public T Source { get; set; }

        public DijkstraResult(T source)
        {
            Source = source;
            Distances = new Dictionary<T, int>();
            Paths = new Dictionary<T, List<T>>();
        }

        public int GetDistance(T vertex)
        {
            return Distances.ContainsKey(vertex) ? Distances[vertex] : int.MaxValue;
        }

        public List<T> GetPath(T vertex)
        {
            return Paths.ContainsKey(vertex) ? Paths[vertex] : new List<T>();
        }

        public bool HasPath(T vertex)
        {
            return Paths.ContainsKey(vertex) && Paths[vertex].Count > 0;
        }

        public void DisplayResults()
        {
            Console.WriteLine($"Dijkstra Results from source: {Source}");
            Console.WriteLine("Vertex\t\tDistance\tPath");
            Console.WriteLine("------\t\t--------\t----");

            foreach (var vertex in Distances.Keys)
            {
                string distance = Distances[vertex] == int.MaxValue ? "∞" : Distances[vertex].ToString();
                string path = HasPath(vertex) ? string.Join(" → ", Paths[vertex]) : "No path";
                
                Console.WriteLine($"{vertex}\t\t{distance}\t\t{path}");
            }
        }
    }

    public static class DijkstraAlgorithm
    {
        public static DijkstraResult<T> FindShortestPaths<T>(Graph<T> graph, T source)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));
            
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (graph.IsEmpty())
                throw new InvalidOperationException("Graph is empty");

            var vertices = graph.GetVertices().ToList();
            
            if (!vertices.Contains(source))
                throw new ArgumentException("Source vertex not found in graph");

            var result = new DijkstraResult<T>(source);
            var distances = new Dictionary<T, int>();
            var visited = new Dictionary<T, bool>();
            var previous = new Dictionary<T, T>();

            foreach (var vertex in vertices)
            {
                distances[vertex] = int.MaxValue;
                visited[vertex] = false;
                previous[vertex] = default(T);
            }

            distances[source] = 0;

            for (int count = 0; count < vertices.Count - 1; count++)
            {
                T minVertex = FindMinimumDistanceVertex(distances, visited);
                
                if (minVertex == null || distances[minVertex] == int.MaxValue)
                    break; 

                visited[minVertex] = true;

                UpdateAdjacentVertices(graph, minVertex, distances, visited, previous);
            }

            BuildResult(result, distances, previous, vertices);

            return result;
        }

        private static T FindMinimumDistanceVertex<T>(Dictionary<T, int> distances, Dictionary<T, bool> visited)
        {
            int minDistance = int.MaxValue;
            T minVertex = default(T);

            foreach (var vertex in distances.Keys)
            {
                if (!visited[vertex] && distances[vertex] <= minDistance)
                {
                    minDistance = distances[vertex];
                    minVertex = vertex;
                }
            }

            return minVertex;
        }

        private static void UpdateAdjacentVertices<T>(Graph<T> graph, T currentVertex, 
            Dictionary<T, int> distances, Dictionary<T, bool> visited, Dictionary<T, T> previous)
        {
            var allVertices = graph.GetVertices().ToList();

            foreach (var vertex in allVertices)
            {
                if (!visited[vertex] && graph.HasEdge(currentVertex, vertex))
                {
                    int edgeWeight = graph.GetEdgeWeight(currentVertex, vertex);
                    int newDistance = distances[currentVertex] + edgeWeight;

                    if (distances[currentVertex] != int.MaxValue && newDistance < distances[vertex])
                    {
                        distances[vertex] = newDistance;
                        previous[vertex] = currentVertex;
                    }
                }
            }
        }

        private static void BuildResult<T>(DijkstraResult<T> result, Dictionary<T, int> distances, 
            Dictionary<T, T> previous, List<T> vertices)
        {
            result.Distances = new Dictionary<T, int>(distances);

            foreach (var vertex in vertices)
            {
                if (distances[vertex] != int.MaxValue)
                {
                    var path = BuildPath(previous, result.Source, vertex);
                    result.Paths[vertex] = path;
                }
            }
        }

        private static List<T> BuildPath<T>(Dictionary<T, T> previous, T source, T destination)
        {
            var path = new List<T>();
            T current = destination;

            while (current != null && !current.Equals(default(T)))
            {
                path.Insert(0, current);
                
                if (current.Equals(source))
                    break;
                    
                current = previous[current];
            }

            if (path.Count > 0 && path[0].Equals(source))
                return path;
            else
                return new List<T>();
        }
        
        public static int GetShortestDistance<T>(Graph<T> graph, T source, T destination)
        {
            var result = FindShortestPaths(graph, source);
            return result.GetDistance(destination);
        }

        public static List<T> GetShortestPath<T>(Graph<T> graph, T source, T destination)
        {
            var result = FindShortestPaths(graph, source);
            return result.GetPath(destination);
        }
    }
}