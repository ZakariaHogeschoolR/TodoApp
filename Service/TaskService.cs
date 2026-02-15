using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using Model;

public class TaskService: ITaskService, IMyCollection<TaskItem>//, IMyIterator<TaskItem>
{
    private readonly ITaskRepository _repository;
    private TaskItem[] _tasks;
    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _tasks = _repository.LoadTasks();
    }

    public IEnumerable<TaskItem> GetAllTasks() => _tasks;
    
    public void AddTask(string description)
    {
        int id = _tasks.Length + 1;
        TaskItem newTask = new TaskItem
        {
            Id = id, 
            Description = description, 
            Completed = false
        };
        Add(newTask);
    }

    public void Add(TaskItem item)
    {
        int size = _tasks.Length + 1;
        TaskItem[] newTaskArray = new TaskItem[size];
        for(int i = 0; i < newTaskArray.Length; i++)
        {
            if(i > _tasks.Length - 1)
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
        _repository.SaveTasks(newTaskArray);
    }

    public TaskItem FindBy<K>(K key, Func<TaskItem, K, bool> comparer)
    {
        for(int i = 0; i < _tasks.Length; i++)
        {
            if(comparer(_tasks[i], key))
            {
                return _tasks[i];
            }
        }
        return null;
    }

    public void RemoveTask(int id)
    {
        var task = FindBy(id, (t, id) => t.Id == id);
        if(task != null)
        {
            Remove(task);
        }
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

        TaskItem[] newArray = new TaskItem[_tasks.Length - 1];
        
        for (int i = 0, j = 0; i < _tasks.Length; i++)
        {
            if (i == index) continue; // overslaan
            newArray[j] = _tasks[i];
            j++;
        }
        _tasks = newArray;
        _repository.SaveTasks(newArray);
    }

    public void ToggleTaskCompletion(int id)
    {

        var task = FindBy(id, (t, id) => t.Id == id);
        if (task != null) {
            int index = -1;
            for(int i = 0; i < _tasks.Length - 1; i++)
            {
                if(_tasks[i].Id == id)
                {
                    index = i;
                    if(_tasks[index].Completed)
                    {
                        _tasks[index].Completed = false;
                    }
                    else
                    {
                        _tasks[index].Completed = true;
                    }
                    _repository.SaveTasks(_tasks);
                    break;
                }
            }
        }
    }
}