namespace EYB.MVVM
{
    public interface IRepository<T>
    {
        ReadOnlyList<T> Items { get; }
        void Delete(T item);
        void Update(T item);
        void Add(T item);
    }
}
