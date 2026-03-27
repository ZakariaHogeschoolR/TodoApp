using System.Text.Json;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;
using Model;

public class JsonTaskRowRepository
{
    public JsonTaskRowRepository()
    {
        
    }
    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Tasks");
        Directory.CreateDirectory(folder);
        var array = tasks.ToArray();
        for(int i = 0; i < tasks.Count; i++)
        {
            string filePath = Path.Combine(folder, $"task_{array[i].Id}.json");
            if(!array[i].changed && File.Exists(filePath))
                continue;
            string jsonRow = JsonSerializer.Serialize(array[i], new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonRow);
            array[i].changed = false;
        }
    }
}