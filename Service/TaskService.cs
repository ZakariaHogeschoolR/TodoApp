using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using Model;

public class TaskService: ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly TaskArray _tasks;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _tasks = new TaskArray(_repository.LoadTasks());
    }

    public IEnumerable<TaskItem> GetAllTasks() => _tasks.ToArray();
    public IEnumerable<TaskItem> GetAllViewTasks() => _tasks.ToViewArray();
    
    public void AddTask(string description)
    {
        int id = _tasks.ToArray().Length + 1;
        TaskItem newTask = new TaskItem
        {
            Id = id, 
            Description = description, 
            Completed = false,
            Status = statusProgression.ToDo
        };
        _tasks.Add(newTask);
        _repository.SaveTasks(_tasks.ToArray());
    }

    public void UpdateTask(string description, int id)
    {
        var task = _tasks.FindBy(id, (t, id) => t.Id == id);
        _tasks.Update(description, task);
        _repository.SaveTasks(_tasks.ToArray());
    }

    public void RemoveTask(int id)
    {
        var task = _tasks.FindBy(id, (t, id) => t.Id == id);
        if(task != null)
        {
            _tasks.Remove(task);
            _repository.SaveTasks(_tasks.ToArray());
        }
    }

    public void ToggleTaskCompletion(int id)
    {

        var task = _tasks.FindBy(id, (t, id) => t.Id == id);
        if (task != null) {
            int index = -1;
            for(int i = 0; i < _tasks.ToArray().Length; i++)
            {
                if(_tasks.ToArray()[i].Id == id)
                {
                    index = i;
                    if(_tasks.ToArray()[index].Completed)
                    {
                        _tasks.ToArray()[index].Completed = false;
                    }
                    else
                    {
                        _tasks.ToArray()[index].Completed = true;
                    }
                    _repository.SaveTasks(_tasks.ToArray());
                    break;
                }
            }
        }
    }
    public void SortByStatus()
    {
        _tasks.SortByStatus();
    }
    public void ChangeStatus(int id, int status)
    {
        for(int i = 0; i < _tasks.ToArray().Length; i++)
        {
            if(_tasks.ToArray()[i].Id  == id)
            {
                _tasks.ToArray()[i].Status = (statusProgression)status;
            }
        }
        _repository.SaveTasks(_tasks.ToArray());
    }

    public IMyCollection<TaskItem> FilterByStatus(statusProgression status)
    {
        TaskArray filtered = (TaskArray)_tasks.Filter(t => t.Status == status);
        return  new TaskArray(filtered.ToArray());
    } 

    public void List(IMyCollection<TaskItem> collection, statusProgression status )
    {
        Console.Clear();
        Console.WriteLine("=============      ToDo List      =============");
        Console.WriteLine("|---------------------------------------------|");
        Console.WriteLine($"|  {status}  |");
        foreach(TaskItem item in collection)
        {
            Console.WriteLine($"| {item.Description}: {item.Status} |");
        }
    }
}