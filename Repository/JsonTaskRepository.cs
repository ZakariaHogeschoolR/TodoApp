using System.Text.Json;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;
using Model;

public class JsonTaskRepository: ITaskRepository 
{
    private readonly string _filePath;
    public JsonTaskRepository(string filePath) => _filePath = filePath;
    public IMyCollection<TaskItem> LoadTasks()
    {
        if(!File.Exists(_filePath))
        {
            return new Array<TaskItem>(new TaskItem[0]);
        }
        string json = File.ReadAllText(_filePath);
        var tasks = JsonSerializer.Deserialize<TaskItem[]>(json);
        return new Array<TaskItem>(tasks) ?? new Array<TaskItem>(new TaskItem[tasks.Length]);
    }
    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        var array = tasks.ToArray();
        string json = JsonSerializer.Serialize(array, new
            JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json); 
    }
}