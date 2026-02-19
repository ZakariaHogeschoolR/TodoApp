using System.Linq.Expressions;
using System.Security.Authentication.ExtendedProtection;
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
        IEnumerable<TaskItem> tasks = _service.GetAllViewTasks();
        Console.ReadKey();
        Console.Clear();
        Console.WriteLine("=============      ToDo List      =============");
        Console.WriteLine("|---------------------------------------------|");
        Console.WriteLine("| ToDo       |    InProgress    |     Done    |");

        int index = 0;

        foreach (TaskItem task in tasks)
        {
            string cell = task != null ? task.Description : "";

            if (index % 3 == 0)
                Console.Write($"|  {cell.PadRight(12)}");
            else if (index % 3 == 1)
                Console.Write($"|  {cell.PadRight(14)}");
            else if (index % 3 == 2)
                Console.Write($"|  {cell.PadRight(10)}|");

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
                    Console.Write($"|  {"".PadRight(14)}");
                else if (index % 3 == 2)
                    Console.Write($"|  {"".PadRight(10)}|");
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
            Console.WriteLine("2. Remove Task");
            Console.WriteLine("3. Toggle Task State");
            Console.WriteLine("4. Set status");
            Console.WriteLine("5. Exit");

            string option = Prompt("Select an option: ");
            switch (option)
            {
                case "1":
                    string description  = Prompt("Enter task description: ");
                    _service.AddTask(description);
                    break;
                case "2":
                    string removeStr = Prompt("Enter task id to remove: ");
                    if(int.TryParse(removeStr, out int removeId))
                    {
                        _service.RemoveTask(removeId);
                    }
                    break;
                case "3":
                    string toggleIdStr = Prompt("Enter task id to toggle: ");
                    if(int.TryParse(toggleIdStr, out int toggleId))
                    {
                        _service.ToggleTaskCompletion(toggleId);
                    }
                    break;
                case "4":
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
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}