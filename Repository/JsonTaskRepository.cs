using System.Text.Json;
using Microsoft.VisualBasic;
using Model;

public class JsonTaskRepository: ITaskRepository 
{
    private readonly string _filePath;
    public JsonTaskRepository(string filePath) => _filePath = filePath;
    public TaskItem[] LoadTasks()
    {
        if(!File.Exists(_filePath))
        {
            return new TaskItem[0];
        }
        string json = File.ReadAllText(_filePath);
        var tasks = JsonSerializer.Deserialize<TaskItem[]>(json);
        return tasks ?? new TaskItem[tasks.Length];
    }
    public void SaveTasks(TaskItem[] tasks)
    {
        string json = JsonSerializer.Serialize(tasks, new
            JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json); 
    }
}