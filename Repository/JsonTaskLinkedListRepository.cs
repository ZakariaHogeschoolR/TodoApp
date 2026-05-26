using System.Text.Json;
using Model;
public class JsonTaskLinkedListRepository : ITaskRepository
{
    private readonly string _filePath;

    public JsonTaskLinkedListRepository(string filePath) => _filePath = filePath;

    public IMyCollection<TaskItem> LoadTasks()
    {
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Tasks");
        var linkedList = new LinkedList<TaskItem>();

        if (!Directory.Exists(folder))
        {
            return linkedList;
        }
        string[] files = Directory.GetFiles(folder, "*.json");

        for (int i = 0; i < files.Length; i++)
        {
            string json = File.ReadAllText(files[i]);

            TaskItem? task = JsonSerializer.Deserialize<TaskItem>(json);

            if (task != null)
            {
                linkedList.Add(task);
            }
        }
        return linkedList;
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