using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Model;
using System.Diagnostics.CodeAnalysis;

public class TaskArray<T>: IMyCollection<T>, ITaskArray<T> where T : TaskItem
{
    private class Node
    {
        private T _data;
        public Node? next;
        public Node? prev;
        public Node(T data)
        {
            _data = data;
            next = null;
            prev = null;
        }
    }

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

    public TaskArray(T[] initialTasks = null)
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
        T[] array = new T[Count + 3];
        for(int i = 0; i < Count; i++)
        {
            array[i] = _tasks[i]; // copy existing tasks
        }
        item.Id = Count + 1;
        array[Count + 2] = item; // place new item at end
        _tasks = array;
        Count += 3; // increment after
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
            int rowStart = (index / 3) * 3;

        int itemsInRow = 0;

        for (int i = rowStart; i < rowStart + 3 && i < Count; i++)
        {
            if (_tasks[i] != null)
            {
                itemsInRow++;
            }
        }
        if (itemsInRow == 1)
        {
            T[] newArray = new T[Count - 3];

            for (int i = 0, j = 0; i < Count; i++)
            {
                if (i >= rowStart && i < rowStart + 3)
                    continue;

                newArray[j++] = _tasks[i];
            }

            _tasks = newArray;
            Count -= 3;
        }
        else
        {
            // remove only the item
            _tasks[index] = default;
        }
        _dirty = false;
    }

    public T? FindBy<K>(K Key, Func<T, K, bool> comparer)
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
        public T? Task { get; set; }
        public TaskResult(int index, T? task)
        {
            Index = index;
            Task = task;
        }
    }

    public TaskResult FindByStatus( statusProgression status, bool[] used, int startIndex = 0)
    {
        for (int i = startIndex; i < Count + 1; i++)
        {
            if(_tasks[i] == null)
            {
                continue;
            }
            else
            {
                if (!used[i] && _tasks[i].Status == status)
                {
                    return new TaskResult(i, _tasks[i]);
                }
            }
        }

        return new TaskResult(-1, null);
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
        return new TaskArray<T>(TaskItemArray);
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

    public int MaxDescription()
    {
        T? maxDescription = null;
        for(int i = 0; i < Count; i++)
        {
            if(_tasks[i] == null)
            {
                continue;
            }
            if(maxDescription == null)
            {
                maxDescription = _tasks[i];
            }
            if(maxDescription.Description.Length < _tasks[i].Description.Length)
            {
                maxDescription = _tasks[i];
            }
        }
        if(maxDescription == null)
        {
            return 10;
        }else
        {
            return maxDescription.Description.Length;
        }
    }


    public void SortByStatus()
    {
        if (Count <= 0) return;

        T[] sorted = new T[Count];

        int todoIndex = 0;
        int inProgressIndex = 1;
        int doneIndex = 2;

        for (int i = 0; i < Count; i++)
        {
            T task = _tasks[i];
            if(task == null)
            {
                continue;
            }
            switch (task.Status)
            {
                case statusProgression.ToDo:
                    if (todoIndex < Count)
                    {
                        task.Id = todoIndex;
                        sorted[todoIndex] = task;
                        todoIndex += 3;
                    }
                    break;

                case statusProgression.InProgress:
                    if (inProgressIndex < Count)
                    {
                        task.Id  = inProgressIndex;
                        sorted[inProgressIndex] = task;
                        inProgressIndex += 3;
                    }
                    break;

                case statusProgression.Done:
                    if (doneIndex < Count)
                    {
                        task.Id = doneIndex;
                        sorted[doneIndex] = task;
                        doneIndex += 3;
                    }
                    break;
            }
        }
        Count = sorted.Length;
        _tasks = sorted;
        Dirty = false;
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