using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Model;
using System.Diagnostics.CodeAnalysis;

public class UsersArray<T>: IMyCollection<T> where T : Users
{
    private T[] _tasks;
    private int _count;
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
    public bool Dirty
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

    public UsersArray(T[] initialTasks = null)
    {
        if (initialTasks != null)
        {
            _tasks = initialTasks.ToArray();
        }
        else
        {
            _tasks = new T[0];
        }
        _count = _tasks.Length;
        _dirty = true;
    }

    public void Add(T item)
    {
        if (item == null) return;
        T[] array = new T[Count + 1];
        for(int i = 0; i < Count; i++)
        {
            array[i] = _tasks[i]; // copy existing tasks
        }
        item.Id = Count + 1;
        array[Count] = item; // place new item at end
        _tasks = array;
        Count += 1; // increment after
        _dirty = true;
    }

    public void Update(T change, T item)
    {
        for(int i = 0; i < Count; i++)
        {
            if(_tasks[i] == item)
            {
                _tasks[i] = change;
            }
        }
        _dirty = true;
    }

    public void Remove(T Item)
    {
        if(Item == null) return;

        int index = -1;

        for (int i = 0; i < Count + 1; i++)
        {
            if (_tasks[i] == Item)
            {
                index = i;
                break;
            }
        }
        if(index == -1) return;

        T[] newArray = new T[Count];
        
        for (int i = 0, j = 0; i < Count; i++)
        {
            if (i == index) continue; // overslaan
            newArray[j] = _tasks[i];
            j++;
        }
        _tasks = newArray;
        _dirty = false;
    }

    public T FindBy<K>(K Key, Func<T, K, bool> comparer)
    {
        for(int i = 0; i < Count; i++)
        {
            if(_tasks[i] == null)
            {
                continue;
            }
            if(comparer(_tasks[i], Key))
            {
                return _tasks[i];
            }
        }
        return null;
    }

    public struct TaskResult
    {
        public int Index { get; set;}
        public T Task { get; set; }
        public TaskResult(int index, T task)
        {
            Index = index;
            Task = task;
        }
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        T[] temporaryArray = new T[Count + 1];
        int size = 0;
        for(int i = 0; i < Count; i++)
        {
            if(_tasks[i] == null)
            {
                continue;
            }
            if(predicate(_tasks[i]))
            {
                temporaryArray[size] = _tasks[i];
                size++;
            }
        }
        T[] TaskItemArray = new T[size];
        for(int j = 0; j < TaskItemArray.Length; j++)
        {
            TaskItemArray[j] = temporaryArray[j];
        }
        return new UsersArray<T>(TaskItemArray);
    }

    public void Sort(Comparison<T> comparison)
    {
        for(int i = 0; i < Count; i++)
        {
            if(i != Count  - 1)
            {
                if(comparison(_tasks[i], _tasks[i + 1]) > 0)
                {
                    T oldOne = _tasks[i];
                    _tasks[i] = _tasks[i + 1];
                    _tasks[i + 1] = oldOne;    
                }
            }
        }
        _dirty = false;
    }
    
    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        if(Count + 1 == 0 )
        {
            throw new InvalidOperationException("Cabbit reduce empty collection without initial value.");
        }
        R result = (R)(object)_tasks[0];
        for(int i = 1; i < Count + 1; i++)
        {
            result = accumulator(result, _tasks[i]);
        }
        return result;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;

        for (int i = 0; i < Count + 1; i++)
        {
            result = accumulator(result, _tasks[i]);
        }

        return result;
    }

    public IMyIterator<T> GetIterator()
    {
       return new TaskArrayIterator<T>(_tasks);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for(int i = 0; i< _tasks.Length; i++)
        {
            yield return _tasks[i];
        }
    }

    IEnumerator<T> IMyCollection<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T[] ToArray()
    {
        T[] copy = new T[Count];
        for (int i = 0; i < Count; i++)
        {
            copy[i] = _tasks[i];
        }
        return copy;
    }
}