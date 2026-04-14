using System.Net;
public class Program
{
    public static void Main(string[] args)
    {
        // 1. Laad de instellingen (onthoudt ARRAY of LINKEDLIST)
        AppSettings.Load();

        string filePathTask = "tasks.json";
        string filePathUser = "users.json";

        // 2. DI: Kies de repository op basis van de statische AppSettings
        ITaskRepository repositoryTask;
        IUserRepository repositoryUser;
        if (AppSettings.Mode == "LINKEDLIST") {
            repositoryTask = new JsonTaskLinkedListRepository(filePathTask);
            repositoryUser = new JsonUserLinkedListRepository(filePathUser);
        }
        else if(AppSettings.Mode == "HASHMAP")
        {
            repositoryTask = new JsonTaskHashMapRepository(filePathTask);
            repositoryUser = new JsonUserHashMapRepository(filePathUser);
        } 
        else {
            repositoryTask = new JsonTaskRepository(filePathTask);
            repositoryUser = new JsonUserRepository(filePathUser);
        }
        JsonTaskRowRepository repositoryTaskRow = new JsonTaskRowRepository();
        JsonUserRowRepository repositoryUserRow = new JsonUserRowRepository();

        IUserService serviceUser = new UserService(repositoryUser);
        ITaskService serviceTasks = new TaskService(repositoryTask);

        // Geef de view alles mee
        ITaskView view = new ConsoleTaskView(serviceTasks, serviceUser, repositoryTaskRow, repositoryUserRow);
        view.Run();
    }
}