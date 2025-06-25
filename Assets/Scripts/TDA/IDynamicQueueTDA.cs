namespace TDA
{
    public interface IDynamicQueueTDA<T>
    {
        bool Enqueue(T item);
        T Dequeue();
        bool IsEmpty();
        T Front();
        void PrintQueue();
    }
    
}
