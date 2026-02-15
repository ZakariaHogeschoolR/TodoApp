using System.Runtime.CompilerServices;
using Model;
public interface ITaskRepository
{
    TaskItem[] LoadTasks();
    void SaveTasks(TaskItem[] tasks);
}