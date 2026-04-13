using System.Text.Json;
using Model;
public class JsonTaskLinkedListRepository : ITaskRepository
{
    private readonly string _filePath;

    public JsonTaskLinkedListRepository(string filePath) => _filePath = filePath;

    public IMyCollection<TaskItem> LoadTasks()
    {
        // CRUCIAAL: We maken hier jouw specifieke LinkedList aan
        var linkedList = new LinkedList<TaskItem>();

        if (!File.Exists(_filePath)) return linkedList;

        string json = File.ReadAllText(_filePath);
        var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);

        if (tasks != null)
        {
            foreach (var task in tasks)
            {
                // De .Add() methode van je LinkedList maakt intern de Nodes aan
                linkedList.Add(task);
            }
        }
        return linkedList;
    }

    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        // We gebruiken ToArray() of ToList() om de ketting weer plat te slaan voor JSON
        var json = JsonSerializer.Serialize(tasks.ToArray(), new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}