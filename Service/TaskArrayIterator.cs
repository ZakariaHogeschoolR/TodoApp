using Model;
public class TaskArrayIterator: IMyIterator<TaskItem>
{
    private TaskItem[] _tasks;
    private int _index;

    public TaskArrayIterator(TaskItem[] tasks)
    {
        _tasks = tasks;
        _index = 0;
    }

    public bool HasNext()
    {
        return _index < _tasks.Length;
    }
    public TaskItem Next()
    {
        if(HasNext())
        {
            _index++;
            return _tasks[_index];
        }
        return default(TaskItem);
    }
    public void Reset()
    {
        _index = 0;
    }

}