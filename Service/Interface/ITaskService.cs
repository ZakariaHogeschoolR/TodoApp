using Model;

public interface ITaskService
{
    IEnumerable<TaskItem> GetAllTasks();
    IEnumerable<TaskItem> GetAllViewTasks();
    void AddTask(string description);
    void UpdateTask(string description, int id);
    void RemoveTask(int id);
    IMyCollection<TaskItem> FilterByStatus(statusProgression status);
    void ToggleTaskCompletion(int id);
    void ChangeStatus(int id, int status);
    void SortByStatus();
    void List(IMyCollection<TaskItem> collection, statusProgression status);

}