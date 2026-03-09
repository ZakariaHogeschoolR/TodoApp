public class Program
{
    public static void Main(string[] args)
    {
        // filePath of the tasks.......
        string filePathTask = "tasks.json";
        string filePathUser = "users.json";
        ITaskRepository repositoryTask = new JsonTaskRepository(filePathTask);
        IUserRepository repositoryUser = new JsonUserRepository(filePathUser);
        ITaskService serviceTasks = new TaskService(repositoryTask);
        IUserService serviceUser = new UserService(repositoryUser);
        ITaskView view = new ConsoleTaskView(serviceTasks, serviceUser);

        view.Run();
    }
}
