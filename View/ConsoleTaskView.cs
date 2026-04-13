using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Channels;
using System.Diagnostics.CodeAnalysis;
using Model;
using System.Transactions;
using Microsoft.VisualBasic;

public class ConsoleTaskView: ITaskView
{
    public static int count = 0;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    JsonTaskRowRepository taskRepo;
    JsonUserRowRepository userRepo;
    private System.Timers.Timer timer;
    public ConsoleTaskView(ITaskService taskService, IUserService userService, JsonTaskRowRepository taskRow, JsonUserRowRepository userRow)
    {
        _taskService = taskService;
        _userService = userService;
        taskRepo = taskRow;
        userRepo = userRow;
        timer = new System.Timers.Timer(5000);
    }

    public void DisplayTasks()
    {
        IMyCollection<TaskItem> tasks = _taskService.GetAllTasks();
        if(tasks.Dirty)
        {
            _taskService.SortByStatus();
        }
        int maxDescription = _taskService.MaxDescription();
        Console.Clear();
        Console.WriteLine($"[ Systeem Modus: {AppSettings.Mode} ]");
        Console.Write(new string('=', maxDescription + maxDescription - 13));
        Console.Write("      ToDo List      ");
        Console.Write(new string('=', maxDescription + maxDescription - 12));
        Console.WriteLine();
        Console.Write("|"); 
        Console.Write(new string('=', maxDescription + maxDescription - 3));
        Console.Write(new string('=', maxDescription + maxDescription - 3));
        Console.Write("|");
        Console.WriteLine();
        Console.WriteLine($"|{"".PadRight(maxDescription / 2 + 1)}ToDo{"".PadRight(maxDescription / 2 - 1)}|{"".PadRight(maxDescription / 2 + 1)}InProgress{"".PadRight(maxDescription / 2 - 1)}|{"".PadRight(maxDescription / 2 + 1)}Done{"".PadRight(maxDescription / 2 - 1)}|");

        int index = 0;
        TaskItem[] todo       = new TaskItem[tasks.Count];
        TaskItem[] inProgress = new TaskItem[tasks.Count];
        TaskItem[] done       = new TaskItem[tasks.Count];
        int todoCount = 0, inProgressCount = 0, doneCount = 0;

        foreach (TaskItem task in tasks)
        {
            if (task == null) continue;
            if      (task.Status == statusProgression.ToDo)       todo[todoCount++]             = task;
            else if (task.Status == statusProgression.InProgress) inProgress[inProgressCount++] = task;
            else if (task.Status == statusProgression.Done)       done[doneCount++]             = task;
        }

        int rowCount = Math.Max(todoCount, Math.Max(inProgressCount, doneCount));
        if (rowCount == 0) rowCount = 1;

        TaskItem[] ordered = new TaskItem[rowCount * 3];
        for (int i = 0; i < rowCount; i++)
        {
            ordered[i * 3]     = i < todoCount       ? todo[i]       : null;
            ordered[i * 3 + 1] = i < inProgressCount ? inProgress[i] : null;
            ordered[i * 3 + 2] = i < doneCount       ? done[i]       : null;
        }
        foreach (TaskItem task in ordered)
        {
            bool isNull = task == null;
            string cell = task != null ? task.Description : "";
            int showId = task == null ? 0 : task.showId;
            if(showId == 0 && task == null)
            {
                isNull = true;
            }
            if (index % 3 == 0)
            {
                if(isNull)
                {
                    Console.Write($"|   {cell.PadRight(maxDescription + 1)}");
                }
                else
                {
                    Console.Write($"|{showId}. {cell.PadRight(maxDescription + 1)}");
                }
            }
            else if (index % 3 == 1)
            {
                if(isNull)
                {
                    Console.Write($"|   {cell.PadRight(maxDescription + 7)}");
                }
                else
                {
                    Console.Write($"|{showId}. {cell.PadRight(maxDescription + 7)}");
                }
            }
            else if (index % 3 == 2)
            {
                if(isNull)
                {
                    Console.Write($"|   {cell.PadRight(maxDescription + 1)}|");
                }
                else
                {
                    Console.Write($"|{showId}. {cell.PadRight(maxDescription + 1)}|");
                }
            }
            index++;

            if (index % 3 == 0)
                Console.WriteLine();
        }

        if (index % 3 != 0)
        {
            // Fill remaining columns in the last incomplete row
            while (index % 3 != 0)
            {
                if (index % 3 == 1)
                    Console.Write($"|  {"".PadRight(maxDescription)}");
                else if (index % 3 == 2)
                    Console.Write($"|  {"".PadRight(maxDescription)}|");
                index++;
            }
            Console.WriteLine();
        }
    }

    
    string Prompt(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    public void Run()
    {
        timer.Elapsed += (s, e) =>
        {
            // Get live tasks from service
            IMyCollection<TaskItem> tasks = _taskService.GetAllTasks();
            taskRepo.SaveTasks(tasks);
        };
        timer.AutoReset = true;
        timer.Start();
        while(true)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Please Log In");
            Console.WriteLine("-----------------");
            string username = Prompt("Please enter your Username: ");
            string password = Prompt("Please enter your Password: ");
            if(_userService.CheckCredentials(username, password))
            {
                while (true)
                {
                    DisplayTasks();
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("1. Add Task");
                    Console.WriteLine("2. Update Task");
                    Console.WriteLine("3. Remove Task");
                    Console.WriteLine("4. Toggle Task State");
                    Console.WriteLine("5. Set status");
                    Console.WriteLine("6. Filter status");
                    Console.WriteLine("7. Assign to an task");
                    Console.WriteLine("8. See participants of an task");
                    Console.WriteLine("9. Log out");
                    Console.WriteLine("10. swap DataStructure");
                    Console.WriteLine("11. Exit");

                    string option = Prompt("Select an option: ");
                    switch (option)
                    {
                        case "1":
                            _taskService.SortByStatus();
                            string description  = Prompt("Enter task description: ");
                            int priority = Convert.ToInt32(Prompt("Enter an int for priority: "));
                            _taskService.AddTask(description, priority);
                            _taskService.SortByStatus();
                            break;
                        case "2":
                            _taskService.SortByStatus();
                            int updateInt = Convert.ToInt32(Prompt("Enter task id to Update: "));
                            string updateDescription = Prompt("Enter new description: ");
                            _taskService.UpdateTask(updateDescription, updateInt);
                            _taskService.SortByStatus();
                            break;
                        case "3":
                            _taskService.SortByStatus();
                            string removeStr = Prompt("Enter task id to remove: ");
                            if(int.TryParse(removeStr, out int removeId))
                            {
                                _taskService.RemoveTask(removeId);
                            }
                            _taskService.SortByStatus();
                            break;
                        case "4":
                            _taskService.SortByStatus();
                            string toggleIdStr = Prompt("Enter task id to toggle: ");
                            if(int.TryParse(toggleIdStr, out int toggleId))
                            {
                                _taskService.ToggleTaskCompletion(toggleId);
                            }
                            _taskService.SortByStatus();
                            break;
                        case "5":
                            _taskService.SortByStatus();
                            int IdStr = Convert.ToInt32(Prompt("Enter task id: "));
                            Console.WriteLine("1. To Do");
                            Console.WriteLine("2. In Progress");
                            Console.WriteLine("3. Done");
                            int Status = Convert.ToInt32(Prompt("Enter task status id: "));
                            switch(Status)
                            {
                                case 1:
                                    _taskService.ChangeStatus(IdStr, Status);
                                    break;
                                case 2:
                                    _taskService.ChangeStatus(IdStr, Status);
                                    break;
                                case 3:
                                    _taskService.ChangeStatus(IdStr, Status);
                                    break;
                                default:
                                    Console.WriteLine("Invalid option. Press any key to continue...");
                                    Console.ReadKey();
                                    break;
                            }
                            _taskService.SortByStatus();
                            break;
                        case "6":
                            _taskService.SortByStatus();  
                            int index = 0;
                            foreach(statusProgression status in Enum.GetValues(typeof(statusProgression)))
                            {
                                index++;
                                Console.WriteLine($"{index}. " + status);
                            }
                            int prompt = Convert.ToInt32(Prompt("Enter choice to filter: "));
                            statusProgression chosen = (statusProgression)Enum.GetValues(typeof(statusProgression)).GetValue(prompt - 1);
                            IMyCollection<TaskItem> array = _taskService.FilterByStatus(chosen);
                            _taskService.List(array, chosen);
                            Console.ReadKey();
                            _taskService.SortByStatus();                 
                            break;
                        case "7":
                            _taskService.SortByStatus();
                            int id = Convert.ToInt32(Prompt("Enter task id: "));
                            IMyCollection<TaskItem> task = _taskService.GetAllTasks();
                            TaskItem taskItem = task.FindBy(id, (task, id) => task.showId == id);
                            _taskService.AddTeamMembers(taskItem, _userService.CurrentUser);
                            _taskService.SortByStatus();
                            break;
                        case "8":
                            _taskService.SortByStatus();
                            int taskId = Convert.ToInt32(Prompt("Enter TaskId: "));
                            _taskService.SeeParticipants(taskId);
                            Console.ReadKey();
                            _taskService.SortByStatus();  
                            break;
                        case "9":
                            string choice = Prompt("Do you want to log back in with an different Account? "); 
                            if(choice == "y" || choice == "y" || choice =="yes" || choice == "ye")
                            {
                                Console.Clear();
                                Console.WriteLine("");
                                Console.WriteLine("");
                                Console.WriteLine("Please Log In");
                                Console.WriteLine("-----------------");
                                username = Prompt("Please enter your Username: ");
                                password = Prompt("Please enter your Password: ");
                                if(_userService.CheckCredentials(username, password))
                                {
                                    continue;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("");
                                    Console.WriteLine("");
                                    Console.WriteLine("Please register an account");
                                    string Username = Prompt("Enter your Username: ");
                                    string Password = Prompt("Enter your Password: ");
                                    string RepeatPassword = Prompt("Enter your Password agian: ");
                                    _userService.AddUser(Username, Password, RepeatPassword);   
                                }
                            }
                            break;
                        case "10":
                            Console.Clear();
                            Console.WriteLine("=== SYSTEEM MIGRATIE ===");
                            Console.WriteLine("Beschikbare modi: ARRAY, LINKEDLIST, HASHMAP");
                            string setting = Prompt("Typ de gewenste modus: ").ToUpper(); // ToUpper voorkomt kleine letter foutjes

                            if (setting == "ARRAY" || setting == "LINKEDLIST" || setting == "HASHMAP")
                            {
                                AppSettings.Mode = setting;
                                AppSettings.Save();
                                Console.WriteLine($"Modus succesvol aangepast naar: {AppSettings.Mode}");
                            }
                            else
                            {
                                Console.WriteLine("Ongeldige keuze. Er is niets veranderd.");
                            }
                            Console.WriteLine("Druk op een toets om af te sluiten...");
                            Console.ReadKey();
                            Environment.Exit(0);
                            break;
                        case "11":
                            _taskService.SortByStatus();
                            _taskService.SortByStatus();
                            return;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            _taskService.SortByStatus();
                            break;
                    }
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Please register an account");
                string Username = Prompt("Enter your Username: ");
                string Password = Prompt("Enter your Password: ");
                string RepeatPassword = Prompt("Enter your Password agian: ");
                _userService.AddUser(Username, Password, RepeatPassword);   
            }
        }
    }
}