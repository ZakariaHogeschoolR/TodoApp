using Model;

public interface ITaskArray<T>: IMyCollection<T> where T : TaskItem
{
    int MaxDescription();


    void SortByStatus();

    T[] RemoveNull(T[] array);
}