using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;
using Model;

public class TaskService: ITaskService
{
    private static int _idCount = 0;
    public static int IdCount
    {
        get
        {
            return _idCount;
        }
        set
        {
            _idCount += value;
        }
    }
    private readonly ITaskRepository _repository;
    private readonly IMyCollection<TaskItem> _tasks;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _tasks = _repository.LoadTasks(); 
    }

    public IMyCollection<TaskItem> GetAllTasks() => _tasks;
    public void AddTask(string description, int priority)
    {
        int id = _tasks.Count + 1;
        IdCount = 1;
        TaskItem newTask = new TaskItem
        {
            Id = _tasks.Count + 1 % 3, 
            showId = IdCount,
            Description = description, 
            Completed = false,
            Status = statusProgression.ToDo,
            Priority = priority,
            TeamMembersArray = new TeamMembers[0],
            CreatedAt = DateTime.Now
        };
        var taskArray = (TaskArray<TaskItem>)_tasks;
        taskArray.Add(newTask);
        _repository.SaveTasks(taskArray);
    }

    public void UpdateTask(string description, int id)
    {
        var task = _tasks.FindBy(id, (t, id) => t.showId == id);
        TaskItem newTask = new TaskItem
        {
            Id = task.Id, 
            showId = task.showId,
            Description = description, 
            Completed = task.Completed,
            Status = task.Status,
            Priority = task.Priority,
            TeamMembersArray = task.TeamMembersArray,
            CreatedAt = task.CreatedAt
        };
        _tasks.Update(newTask, task);
        _repository.SaveTasks(_tasks);
    }

    public void RemoveTask(int id)
    {
        var task = _tasks.FindBy(id, (t, id) => t.showId == id);
        if(task != null)
        {
            _tasks.Remove(task);
            _repository.SaveTasks(_tasks);
        }
    }

    public void ToggleTaskCompletion(int id)
    {

        var task = _tasks.FindBy(id, (t, id) => t.Id == id);
        if (task != null) {
            int index = -1;
            for(int i = 0; i < _tasks.Count - 1; i++)
            {
                if (_tasks.FindBy(i, (t, i) => t.showId == i) == null)
                {
                    continue;
                }
                if(_tasks.FindBy(i, (t, i) => t.Id == i).showId == id)
                {
                    index = i;
                    if(_tasks.FindBy(index, (t, index) => t.showId == index).Completed)
                    {
                        _tasks.FindBy(index, (t, index) => t.showId == index).Completed = false;
                    }
                    else
                    {
                        _tasks.FindBy(index, (t, index) => t.showId == index).Completed = true;
                    }
                    _repository.SaveTasks(_tasks);
                    break;
                }
            }
        }
    }

    public void SortByStatus()
    {
        
        if (_tasks is ITaskArray<TaskItem> taskArray)
        {
            taskArray.SortByStatus(); // ✅
        }
    }
    
    public void ChangeStatus(int id, int status)
    {
        for(int i = 0; i < _tasks.Count; i++)
        {
            if(_tasks.FindBy(i, (t, i) => t.showId == i) == null)
            {
                continue;
            }
            if(_tasks.FindBy(i, (t, i) => t.showId == i).showId  == id)
            {
                _tasks.FindBy(i, (t, i) => t.showId == i).Status = (statusProgression)status;
            }
        }
        _repository.SaveTasks(_tasks);
    }

    public IMyCollection<TaskItem> FilterByStatus(statusProgression status)
    {
        var filtered = _tasks.Filter(t => t.Status == status);
        return filtered;
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

    public void AddTeamMembers()
    {
        return;
    }
}