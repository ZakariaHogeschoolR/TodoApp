public class Program
{
    public static void Main(string[] args)
    {
        // filePath of the tasks.......
        string filePath = "tasks.json";
        ITaskRepository repository = new JsonTaskRepository(filePath);
        ITaskService service = new TaskService(repository);
        ITaskView view = new ConsoleTaskView(service);

        view.Run();
    }
}
