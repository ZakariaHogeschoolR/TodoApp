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
    
    public void AddTask(string description)
    {
        int id = _tasks.ToArray().Length + 1;
        TaskItem newTask = new TaskItem
        {
            Id = id, 
            Description = description, 
            Completed = false
        };
        _tasks.Add(newTask);
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
}