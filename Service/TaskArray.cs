using Model;

public class TaskArray: IMyCollection<TaskItem>
{
    private TaskItem[] _tasks;
    public int Count
    {
        get
        {
            return _tasks.Length - 1;
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

    public TaskArray(TaskItem[] initialTasks = null)
    {
        _tasks = initialTasks ?? new TaskItem[0];
    }
    public void Add(TaskItem item)
    {
        int size = _tasks.Length + 1;
        TaskItem[] newTaskArray = new TaskItem[size];
        for(int i = 0; i < newTaskArray.Length; i++)
        {
            if(i > Count)
            {
                newTaskArray[size - 1] = item;
                break;
            }
            else
            {
                newTaskArray[i] = _tasks[i];
            }
        }
        _tasks = newTaskArray;
    }

    public void Remove(TaskItem Item)
    {
        if(Item == null) return;

        int index = -1;

        for (int i = 0; i < _tasks.Length; i++)
        {
            if (_tasks[i] == Item)
            {
                index = i;
                break;
            }
        }
        if(index == -1) return;

        TaskItem[] newArray = new TaskItem[Count];
        
        for (int i = 0, j = 0; i < _tasks.Length; i++)
        {
            if (i == index) continue; // overslaan
            newArray[j] = _tasks[i];
            j++;
        }
        _tasks = newArray;
    }

    public TaskItem FindBy<K>(K Key, Func<TaskItem, K, bool> comparer)
    {
        for(int i = 0; i < _tasks.Length; i++)
        {
            if(comparer(_tasks[i], Key))
            {
                return _tasks[i];
            }
        }
        return null;
    }

    public IMyCollection<TaskItem> Filter(Func<TaskItem, bool> predicate)
    {
        TaskItem[] temporaryArray = new TaskItem[_tasks.Length];
        int size = 0;
        for(int i = 0; i < _tasks.Length; i++)
        {
            if(predicate(_tasks[i]))
            {
                temporaryArray[size] = _tasks[i];
                size++;
            }
        }
        TaskItem[] TaskItemArray = new TaskItem[size];
        for(int j = 0; j < TaskItemArray.Length; j++)
        {
            TaskItemArray[j] = temporaryArray[j];
        }
        return new TaskArray(TaskItemArray);
    }

    public void Sort(Comparison<TaskItem> comparison)
    {
        for(int i = 0; i < Count; i++)
        {
            if(i != _tasks.Length - 2)
            {
                if(comparison(_tasks[i], _tasks[i + 1]) > 0)
                {
                    TaskItem oldOne = _tasks[i];
                    _tasks[i] = _tasks[i + 1];
                    _tasks[i + 1] = oldOne;    
                }
            }
        }
    }

    public R Reduce<R>(Func<R, TaskItem, R> accumulator)
    {
        if(_tasks.Length == 0 )
        {
            throw new InvalidOperationException("Cabbit reduce empty collection without initial value.");
        }
        R result = (R)(object)_tasks[0];
        for(int i = 1; i < _tasks.Length; i++)
        {
            result = accumulator(result, _tasks[i]);
        }
        return result;
    }
    public R Reduce<R>(R initial, Func<R, TaskItem, R> accumulator)
    {
        R result = initial;

        for (int i = 0; i < _tasks.Length; i++)
        {
            result = accumulator(result, _tasks[i]);
        }

        return result;
    }

    public IMyIterator<TaskItem> GetIterator()
    {
       return  new TaskArrayIterator(_tasks);
    }
    public IEnumerator<TaskItem> GetEnumerator()
    {
        for(int i = 0; i< _tasks.Length; i++)
        {
            yield return _tasks[i];
        }
    }
    public TaskItem[] ToArray() => _tasks;
}