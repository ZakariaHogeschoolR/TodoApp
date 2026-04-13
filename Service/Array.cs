using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Model;
using System.Diagnostics.CodeAnalysis;

public class Array<T>: IMyCollection<T>
{
    // private class Node
    // {
    //     private T _data;
    //     public Node? next;
    //     public Node? prev;
    //     public Node(T data)
    //     {
    //         _data = data;
    //         next = null;
    //         prev = null;
    //     }
    // }

    private T[] _array;
    private int _count;
    private readonly int _stepSize;
    public int Count
    {
        get
        {
            return _count;
        }
        set
        {
            _count = value;
        }
    } 
    private bool _dirty;
    bool IMyCollection<T>.Dirty
    {
        get
        {
            return _dirty;
        }
        set
        {
            _dirty = value;
        }
    }

    public Array(int stepSize = 1, T[] initialArray = null)
    {
        _stepSize = stepSize;
        if (initialArray != null)
        {
            _array = initialArray.ToArray();
        }
        else
        {
            _array = new T[0];
        }
        _count = _array.Length;
        _dirty = true;
    }

    void IMyCollection<T>.Add(T newTask)
    {
        if (newTask == null) return;

        T[] newArray = new T[_count + _stepSize];

        for (int i = 0; i < _count; i++)
        {
            newArray[i] = _array[i];
        }

        newArray[_count] = newTask;

        _array = newArray;
        _count += _stepSize;
        _dirty = true;
    }


    void IMyCollection<T>.Update(T item, T newItem)
    {
        for (int i = 0; i < _array.Length; i++)
        {
            if (_array[i] != null && _array[i].Equals(item))
            {
                _array[i] = newItem;
                _dirty = true;
                break; 
            }
        }
    }

    void IMyCollection<T>.Remove(T item)
    {
        if (item == null) return;

        // 1. Zoek de index van het item in de interne _array
        int index = -1;
        for (int i = 0; i < _count; i++)
        {
            if (_array[i] != null && _array[i].Equals(item))
            {
                index = i;
                break;
            }
        }

        if (index == -1) return; // Niet gevonden

        // 2. Maak de plek leeg
        _array[index] = default(T);
        _dirty = true;

        // 3. Check of het hele blok (stepSize) nu leeg is
        int rowStart = (index / _stepSize) * _stepSize;
        bool rowIsEmpty = true;
        
        for (int i = rowStart; i < rowStart + _stepSize && i < _array.Length; i++)
        {
            if (_array[i] != null)
            {
                rowIsEmpty = false;
                break;
            }
        }

        // 4. Als het blok leeg is, krimp de array met _stepSize
        if (rowIsEmpty)
        {
            T[] newArray = new T[_array.Length - _stepSize];
            for (int i = 0, j = 0; i < _array.Length; i++)
            {
                // Sla het hele lege blok over
                if (i >= rowStart && i < rowStart + _stepSize) continue;
                
                if (j < newArray.Length)
                {
                    newArray[j] = _array[i];
                    j++;
                }
            }
            _array = newArray;
            _count = _array.Length; // Update de count naar de nieuwe werkelijkheid
        }
    }

    T? IMyCollection<T>.FindBy<TK>(TK Key, Func<T, TK, bool> comparer)
    {
        for(int i = 0; i < Count; i++)
        {
            if(_array[i] == null)
            {
                continue;
            }
            if(comparer(_array[i], Key))
            {
                return _array[i];
            }
        }
        return default(T);
    }

    IMyCollection<T> IMyCollection<T>.Filter(Func<T, bool> predicate)
    {
        T[] temporaryArray = new T[Count + 1];
        int size = 0;
        for(int i = 0; i < Count; i++)
        {
            if(_array[i] == null)
            {
                continue;
            }
            if(predicate(_array[i]))
            {
                temporaryArray[size] = _array[i];
                size++;
            }
        }
        T[] TaskItemArray = new T[size];
        for(int j = 0; j < TaskItemArray.Length; j++)
        {
            TaskItemArray[j] = temporaryArray[j];
        }
        return new Array<T>(_stepSize, TaskItemArray);
    }

    void IMyCollection<T>.Sort(Comparison<T> comparison)
    {
        if (comparison == null)
            throw new ArgumentNullException(nameof(comparison));

        for (int i = 0; i < Count - 1; i++)
        {
            for (int j = 0; j < Count - i - 1; j++)
            {
                if (comparison(_array[j], _array[j + 1]) > 0)
                {
                    T temp = _array[j];
                    _array[j] = _array[j + 1];
                    _array[j + 1] = temp;
                }
            }
        }
        _dirty = false;
    }

    public struct TaskResult
    {
        public int Index { get; set;}
        public T? Task { get; set; }
        public TaskResult(int index, T? task)
        {
            Index = index;
            Task = task;
        }
    }

    R IMyCollection<T>.Reduce<R>(Func<R, T, R> accumulator)
    {
        if(Count + 1 == 0 )
        {
            throw new InvalidOperationException("Cabbit reduce empty collection without initial value.");
        }
        R result = (R)(object)_array[0];
        for(int i = 1; i < Count + 1; i++)
        {
            result = accumulator(result, _array[i]);
        }
        return result;
    }

    R IMyCollection<T>.Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;

        for (int i = 0; i < Count + 1; i++)
        {
            result = accumulator(result, _array[i]);
        }

        return result;
    }

    public IMyIterator<T> GetIterator()
    {
       return new TaskArrayIterator<T>(_array);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for(int i = 0; i< _array.Length; i++)
        {
            yield return _array[i];
        }
    }

    IEnumerator<T> IMyCollection<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    T[] IMyCollection<T>.ToArray()
    {
        T[] copy = new T[Count];
        for (int i = 0; i < Count; i++)
        {
            copy[i] = _array[i];
        }
        return copy;
    }
}