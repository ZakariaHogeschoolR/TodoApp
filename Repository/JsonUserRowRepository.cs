using System.Text.Json;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;
using Model;

public class JsonUserRowRepository
{
    public JsonUserRowRepository()
    {
        
    }
    public void SaveTasks(IMyCollection<Users> users)
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Users");
        Directory.CreateDirectory(folder);
        var array = users.ToArray();
        for(int i = 0; i < users.Count; i++)
        {
            string filePath = Path.Combine(folder, $"user_{array[i].Id}.json");
            if(!array[i].changed && File.Exists(filePath))
                continue;
            string jsonRow = JsonSerializer.Serialize(array[i], new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonRow);
            array[i].changed = false;
        }
    }
}