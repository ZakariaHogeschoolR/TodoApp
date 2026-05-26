using System.Text.Json;
using Model;

public class JsonTaskHashMapRepository : ITaskRepository
{
    private readonly string _filePath;

    public JsonTaskHashMapRepository(string filePath) => _filePath = filePath;

    public IMyCollection<TaskItem> LoadTasks()
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Tasks");
        var taskMap = new HashMap<int, TaskItem>(); 

        if (!Directory.Exists(folder))
        {
            return taskMap;
        }
        string[] files = Directory.GetFiles(folder, "*.json");
        foreach (string file in files)
        {
            try
            {
                string json = File.ReadAllText(file);

                TaskItem? task = JsonSerializer.Deserialize<TaskItem>(json);

                if (task != null)
                {
                    taskMap.Add(task);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij laden van {file}: {ex.Message}");
            }
        }

        return taskMap;
    }

    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Tasks");

        Directory.CreateDirectory(folder);

        var array = tasks.ToArray();

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                continue;

            string filePath = Path.Combine(folder, $"task_{array[i].Id}.json");

            string json = JsonSerializer.Serialize(
                array[i],
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(filePath, json);
        }
    }
}