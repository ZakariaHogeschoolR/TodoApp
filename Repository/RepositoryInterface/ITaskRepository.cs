using System.Runtime.CompilerServices;
using Model;
public interface ITaskRepository
{
    List<TaskItem> LoadTasks();
    void SaveTasks(List<TaskItem> tasks);
}