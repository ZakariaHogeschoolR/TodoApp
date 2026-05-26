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
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Tasks");

        if (!Directory.Exists(folder))
        {
            return new Array<TaskItem>(new TaskItem[0]);
        }

        string[] files = Directory.GetFiles(folder, "*.json");

        TaskItem[] loadedTasks = new TaskItem[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            string json = File.ReadAllText(files[i]);

            TaskItem? task = JsonSerializer.Deserialize<TaskItem>(json);

            if (task != null)
            {
                loadedTasks[i] = task;
            }
        }

        return new Array<TaskItem>(loadedTasks);
    }
    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Tasks");
        Directory.CreateDirectory(folder);
        var array = tasks.ToArray();
        for(int i = 0; i < tasks.Count; i++)
        {
            string filePath = Path.Combine(folder, $"task_{array[i].Id}.json");
            if(File.Exists(filePath))
                continue;
            string jsonRow = JsonSerializer.Serialize(array[i], new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonRow);
            array[i].changed = false;
        }
    }
}