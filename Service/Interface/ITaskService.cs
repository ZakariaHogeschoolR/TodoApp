using Model;

public interface ITaskService
{
    IEnumerable<TaskItem> GetAllTasks();
    IEnumerable<TaskItem> GetAllViewTasks();
    void AddTask(string description);
    void RemoveTask(int id);
    void ToggleTaskCompletion(int id);
    void ChangeStatus(int id, int status);
    void SortByStatus();

}