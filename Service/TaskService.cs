using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;
using Model;
using System.Globalization;
using System.Net;

public class TaskService: ITaskService
{
    private static int _idCount = 1;
    public static int NextId()
    {
        return _idCount++;
    }

    public static void ResetId(int num)
    {
        _idCount -= num;
    }
    // public static int IdPlus
    // {
    //     get
    //     {
    //         return _idCount;
    //     }
    //     set
    //     {
    //         _idCount += value;
    //     }
    // }
    // public static int IdMin
    // {
    //     get
    //     {
    //         return _idCount;
    //     }
    //     set
    //     {
    //         _idCount += value;
    //     }
    // }
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
        //_idCount = _tasks.Count / 3;
        TaskItem newTask = new TaskItem
        {
            Id = _tasks.Count + 1 % 3, 
            showId = NextId(),
            Description = description, 
            Completed = false,
            Status = statusProgression.ToDo,
            Priority = priority,
            TeamMembersArray = new Users[0],
            CreatedAt = DateTime.Now,
            changed = false
        };
        var taskArray = (Array<TaskItem>)_tasks;
        var oldArray = _tasks.ToArray();
        if (newTask == null) return;
        TaskItem[] array = new TaskItem[taskArray.Count + 3];
        for(int i = 0; i < taskArray.Count; i++)
        {
            array[i] = oldArray[i]; // copy existing tasks
        }
        newTask.Id = taskArray.Count + 1;
        array[taskArray.Count + 2] = newTask; // place new item at end
        oldArray = array;
        _tasks.Count += 3; // increment after
        _tasks.Dirty = true;
        _tasks.Add(array);
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
            CreatedAt = task.CreatedAt,
            changed = true
        };
        var array = _tasks.ToArray();
        for(int i = 0; i < _tasks.Count; i++)
        {
            if(array[i] == task)
            {
                array[i] = newTask;
            }
        }
        _tasks.Dirty = true;
        _tasks.Update(array);
        _repository.SaveTasks(_tasks);
    }

    public void RemoveTask(int id)
    {
        var task = _tasks.FindBy(id, (t, id) => t.showId == id);
        if(task == null) return;

        int index = -1;
        var array = _tasks.ToArray();
        for (int i = 0; i < _tasks.Count + 1; i++)
        {
            if (array[i] == task)
            {
                index = i;
                break;
            }
        }

        if(index == -1) return;
            int rowStart = (index / 3) * 3;

        int itemsInRow = 0;

        for (int i = rowStart; i < rowStart + 3 && i < _tasks.Count; i++)
        {
            if (array[i] != null)
            {
                itemsInRow++;
            }
        }
        if (itemsInRow == 1)
        {
            TaskItem[] newArray = new TaskItem[_tasks.Count - 3];

            for (int i = 0, j = 0; i < _tasks.Count; i++)
            {
                if (i >= rowStart && i < rowStart + 3)
                    continue;

                newArray[j++] = array[i];
            }

            array = newArray;
            _tasks.Count -= 3;
        }
        else
        {
            // remove only the item
            array[index] = default;
        }
        if (_tasks.Count <= 1)
        {
            return;
        }
        else
        {
            ResetId(1);
        }
        _tasks.Dirty = false;
        _tasks.Remove(array);
        _repository.SaveTasks(_tasks);
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
        _tasks.Sort((a, b) =>
            a == null && b == null ? 0 :
            a == null ? 1 :
            b == null ? -1 :
            a.Status.CompareTo(b.Status)
        );
        int showId = 1;
        foreach(var task in _tasks)
        {
            if(task != null)
            {
                task.showId = showId++;
            }
        }

        _tasks.Dirty = false;
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

    public void AddTeamMembers(TaskItem taskTeam, Users currentUser)
    {
        bool duplicate = false;
        TaskItem item = _tasks.FindBy<TaskItem>(taskTeam, (task, taskTeam) => task.showId == taskTeam.showId);
        if(item.TeamMembersArray == null || item.TeamMembersArray.Length <= 0 )
        {
            Users[] team = new Users[1];
            team[0] = currentUser;
            item.TeamMembersArray = team;
            item.changed = true;
        }
        else
        {
            Users[] team = new Users[item.TeamMembersArray.Length + 1];
            for(int i = 0; i < team.Length; i++)
            {
                if(i >=  item.TeamMembersArray.Length)
                {
                    team[i] = currentUser;
                    break;
                }
                if(item.TeamMembersArray[i].Name == currentUser.Name && item.TeamMembersArray[i].Password == currentUser.Password)
                {
                    duplicate = true;
                    break;
                }
                else
                {
                    team[i] = item.TeamMembersArray[i];
                }
            }
            if(duplicate == false)
            {
                item.TeamMembersArray = team;
                item.changed = true;
            }
        }
        _repository.SaveTasks(_tasks);
    }

    public int MaxDescription()
    {
        TaskItem? maxDescription = null;
        var array = _tasks.ToArray();
        for(int i = 0; i < _tasks.Count; i++)
        {
            if(array[i] == null)
            {
                continue;
            }
            if(maxDescription == null)
            {
                maxDescription = array[i];
            }
            if(maxDescription.Description.Length < array[i].Description.Length)
            {
                maxDescription = array[i];
            }
        }
        if(maxDescription == null)
        {
            return 10;
        }else if(maxDescription.Description.Length <= 10)
        {
            return 10;
        }
        else
        {
            return maxDescription.Description.Length;
        }
    }
}