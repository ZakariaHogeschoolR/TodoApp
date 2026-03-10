using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Channels;
using System.Diagnostics.CodeAnalysis;
using Model;
using System.Transactions;

public class ConsoleTaskView: ITaskView
{
    public static int count = 0;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    public ConsoleTaskView(ITaskService taskService, IUserService userService)
    {
        _taskService = taskService;
        _userService = userService;
    }

    public void DisplayTasks()
    {
        IMyCollection<TaskItem> tasks = _taskService.GetAllTasks();
        if(tasks.Dirty)
        {
            _taskService.SortByStatus();
        }
        int maxDescription = ((ITaskArray<TaskItem>)tasks).MaxDescription();
        Console.Clear();
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

        foreach (TaskItem task in tasks)
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
                    Console.Write($"|   {cell.PadRight(maxDescription)}");
                }
                else
                {
                    Console.Write($"|{showId}. {cell.PadRight(maxDescription)}");
                }
            }
            else if (index % 3 == 1)
            {
                if(isNull)
                {
                    Console.Write($"|   {cell.PadRight(maxDescription + 6)}");
                }
                else
                {
                    Console.Write($"|{showId}. {cell.PadRight(maxDescription + 6)}");
                }
            }
            else if (index % 3 == 2)
            {
                if(isNull)
                {
                    Console.Write($"|   {cell.PadRight(maxDescription)}|");
                }
                else
                {
                    Console.Write($"|{showId}. {cell.PadRight(maxDescription)}|");
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
                    Console.WriteLine("7. Add Team members/member");
                    Console.WriteLine("8. Exit");

                    string option = Prompt("Select an option: ");
                    switch (option)
                    {
                        case "1":
                            string description  = Prompt("Enter task description: ");
                            int priority = Convert.ToInt32(Prompt("Enter an int for priority: "));
                            _taskService.AddTask(description, priority);
                            _taskService.SortByStatus();
                            break;
                        case "2":
                            int updateInt = Convert.ToInt32(Prompt("Enter task id to Uodate: "));
                            string updateDescription = Prompt("Enter new description: ");
                            _taskService.UpdateTask(updateDescription, updateInt);
                            _taskService.SortByStatus();
                            break;
                        case "3":
                            string removeStr = Prompt("Enter task id to remove: ");
                            if(int.TryParse(removeStr, out int removeId))
                            {
                                _taskService.RemoveTask(removeId);
                            }
                            _taskService.SortByStatus();
                            break;
                        case "4":
                            string toggleIdStr = Prompt("Enter task id to toggle: ");
                            if(int.TryParse(toggleIdStr, out int toggleId))
                            {
                                _taskService.ToggleTaskCompletion(toggleId);
                            }
                            _taskService.SortByStatus();
                            break;
                        case "5":
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
                            _taskService.AddTeamMembers();
                            _taskService.SortByStatus();
                            break;
                        case "8":
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