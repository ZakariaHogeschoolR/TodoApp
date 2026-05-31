using System.Text.Json;
using Model;

public class JsonTaskBinearySearchTreeRepository: ITaskRepository
{
    private readonly string _filePath;

    public JsonTaskBinearySearchTreeRepository(string filePath)
    {
        _filePath = filePath;
    }
    public IMyCollection<TaskItem> LoadTasks()
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Tasks");
        var tree = new BinearySearchTree<TaskItem>();

        if (!Directory.Exists(folder))
        {
            return tree;
        }
        string[] files = Directory.GetFiles(folder, "*.json");

        for(int i = 0; i < files.Length; i++)
        {
            string json = File.ReadAllText(files[i]);

            TaskItem? task = JsonSerializer.Deserialize<TaskItem>(json);

            if (task != null)
            {
                tree.Add(task);
            }
        }
        return tree;
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