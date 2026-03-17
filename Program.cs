using System.Net;

public class Program
{
    public static void Main(string[] args)
    {
        // filePath of the tasks.......
        string filePathTask = "tasks.json";
        string filePathUser = "users.json";
        ITaskRepository repositoryTask = new JsonTaskRepository(filePathTask);
        IUserRepository repositoryUser = new JsonUserRepository(filePathUser);
        IUserService serviceUser = new UserService(repositoryUser);
        ITaskService serviceTasks = new TaskService(repositoryTask);
        ITaskView view = new ConsoleTaskView(serviceTasks, serviceUser);
        view.Run();
    }
}
