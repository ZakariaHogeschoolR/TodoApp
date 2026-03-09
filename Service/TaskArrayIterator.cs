using Model;
using System.Diagnostics.CodeAnalysis;
public class TaskArrayIterator<T>: IMyIterator<T>
{
    private T[] _tasks;
    private int _index;

    public TaskArrayIterator(T[] tasks)
    {
        _tasks = tasks;
        _index = 0;
    }

    public bool HasNext()
    {
        return _index < _tasks.Length;
    }
    public T Next()
    {
        if(HasNext())
        {
            _index++;
            return _tasks[_index];
        }
        return default(T);
    }
    public void Reset()
    {
        _index = 0;
    }

}