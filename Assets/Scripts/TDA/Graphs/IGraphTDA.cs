using System.Collections.Generic;

using TDA.Sets;

namespace TDA.Graphs
{
    public interface IGraph<T>
    {
        void AddVertex(T vertex);
        bool RemoveVertex(T vertex);
        ISetTDA<T> GetVertices();
        void AddEdge(T source, T destination, int weight = 1);
        bool RemoveEdge(T source, T destination);
        bool HasEdge(T source, T destination);
        int GetEdgeWeight(T source, T destination);
        bool IsEmpty();
        int VertexCount();
        int EdgeCount();
        void Clear();
        void SetComparer(IComparer<T> comparer);

        List<T> GetPath(T source, T destination);

        List<T> DepthFirstSearch(T startVertex);
        List<T> BreadthFirstSearch(T startVertex);

        void DisplayGraph();
    }
}