using System.Text.Json;
using Model;

public class JsonTaskHashMapRepository : ITaskRepository
{
    private readonly string _filePath;

    public JsonTaskHashMapRepository(string filePath) => _filePath = filePath;

    public IMyCollection<TaskItem> LoadTasks()
    {
        // We maken een HashMap met 'int' als Key (het TaskId) en 'TaskItem' als object
        var taskMap = new HashMap<int, TaskItem>(); 

        if (!File.Exists(_filePath)) return taskMap;

        try 
        {
            string json = File.ReadAllText(_filePath);
            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);

            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    taskMap.Add(task);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij laden tasks: {ex.Message}");
        }

        return taskMap;
    }

    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        // ToArray() zorgt dat de buckets worden samengevoegd tot één lijst voor JSON
        var json = JsonSerializer.Serialize(tasks.ToArray(), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}