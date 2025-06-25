namespace TDA
{
    public interface IStackTDA<T>
    {
        void InitializeStack(int capacity);
        bool Push(T item);
        T Pop();
        T Top();
        bool IsEmpty();
        void PrintStack();

    }
}