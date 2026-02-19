using System.Diagnostics.Contracts;
using Model;
public interface IMyCollection<T>
{
    int Count { get; }
    bool Dirty { get; set; }
    void Add(T item);
    void Remove(T item);
    T FindBy<K>(K key, Func<T, K, bool> comparer);
    IMyCollection<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
    R Reduce<R>(Func<R, TaskItem, R> accumulator);
    R Reduce<R>(R initial, Func<R, TaskItem, R> accumulator);
    IMyIterator<TaskItem> GetIterator();
    IEnumerator<TaskItem> GetEnumerator();
}

public interface IMyIterator<T>
{
    bool HasNext();
    TaskItem Next();
    void Reset();
}