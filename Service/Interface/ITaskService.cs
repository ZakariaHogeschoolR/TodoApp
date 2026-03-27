using Model;

public interface ITaskService
{
    IMyCollection<TaskItem> GetAllTasks();
    void AddTask(string description, int priority);
    void UpdateTask(string description, int id);
    void RemoveTask(int id);
    IMyCollection<TaskItem> FilterByStatus(statusProgression status);
    void ToggleTaskCompletion(int id);
    void ChangeStatus(int id, int status);
    void SortByStatus();
    void List(IMyCollection<TaskItem> collection, statusProgression status);
    void AddTeamMembers(TaskItem taskTeam, Users currentUser);
    int MaxDescription();
}