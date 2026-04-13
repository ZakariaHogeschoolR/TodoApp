using System.Diagnostics.Contracts;
using Model;
public interface IMyCollection<T>
{
    int Count { get; set; }
    bool Dirty { get; set; }
    void Add(T newItem);
    void Update(T item, T newItem);
    void Remove(T item);
    T? FindBy<TK>(TK key, Func<T, TK, bool> comparer);
    IMyCollection<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
    R Reduce<R>(Func<R, T, R> accumulator);
    R Reduce<R>(R initial, Func<R, T, R> accumulator);
    IMyIterator<T> GetIterator();
    IEnumerator<T> GetEnumerator();
    T[] ToArray();
}

public interface IMyIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
}