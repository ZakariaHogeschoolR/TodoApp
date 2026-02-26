using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Channels;
using Model;

public class ConsoleTaskView: ITaskView
{
    private readonly ITaskService _service;
    public ConsoleTaskView(ITaskService service)
    {
        _service = service;
    }
    public void DisplayTasks()
    {
        _service.SortByStatus();
        IMyCollection<TaskItem> tasks = _service.GetAllTasks();
        int maxDescription = tasks.MaxDescription();
        Console.Clear();
        Console.Write(new string('=', maxDescription * 1 + 4));
        Console.Write("      ToDo List      ");
        Console.Write(new string('=', maxDescription * 1 + 4));
        Console.WriteLine();
        Console.Write("|"); 
        Console.Write(new string('=', maxDescription * 1 + 14));
        Console.Write(new string('=', maxDescription * 1 + 14));
        Console.Write("|");
        Console.WriteLine();
        Console.WriteLine($"|{"".PadRight(maxDescription / 2)}ToDo{"".PadRight(maxDescription / 2 - 1)}|{"".PadRight(maxDescription / 2)}InProgress{"".PadRight(maxDescription / 2 - 1)}|{"".PadRight(maxDescription / 2)}Done{"".PadRight(maxDescription / 2 - 1)}|");

        int index = 0;

        foreach (TaskItem task in tasks)
        {
            string cell = task != null ? task.Description : "";

            if (index % 3 == 0)
                Console.Write($"|  {cell.PadRight(maxDescription)}");
            else if (index % 3 == 1)
                Console.Write($"|  {cell.PadRight(maxDescription + 6)}");
            else if (index % 3 == 2)
                Console.Write($"|  {cell.PadRight(maxDescription)}|");

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
            Console.WriteLine("7. Exit");

            string option = Prompt("Select an option: ");
            switch (option)
            {
                case "1":
                    string description  = Prompt("Enter task description: ");
                    int priority = Convert.ToInt32(Prompt("Enter an int for priority: "));
                    _service.AddTask(description, priority);
                    _service.SortByStatus();
                    break;
                case "2":
                    int updateInt = Convert.ToInt32(Prompt("Enter task id to Uodate: "));
                    string updateDescription = Prompt("Enter new description: ");
                    _service.UpdateTask(updateDescription, updateInt);
                    _service.SortByStatus();
                    break;
                case "3":
                    string removeStr = Prompt("Enter task id to remove: ");
                    if(int.TryParse(removeStr, out int removeId))
                    {
                        _service.RemoveTask(removeId);
                    }
                    _service.SortByStatus();
                    break;
                case "4":
                    string toggleIdStr = Prompt("Enter task id to toggle: ");
                    if(int.TryParse(toggleIdStr, out int toggleId))
                    {
                        _service.ToggleTaskCompletion(toggleId);
                    }
                    _service.SortByStatus();
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
                            _service.ChangeStatus(IdStr, Status);
                            break;
                        case 2:
                            _service.ChangeStatus(IdStr, Status);
                            break;
                        case 3:
                            _service.ChangeStatus(IdStr, Status);
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                    _service.SortByStatus();
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
                    IMyCollection<TaskItem> array = _service.FilterByStatus(chosen);
                    _service.List(array, chosen);
                    Console.ReadKey(); 
                    _service.SortByStatus();                  
                    break;
                case "7":
                    _service.AddTeamMembers();
                    _service.SortByStatus();
                    break;
                case "8":
                    _service.SortByStatus();
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    _service.SortByStatus();
                    break;
            }
        }
    }
}