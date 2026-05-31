
using Model;

public class BinearySearchTree<T> : IMyCollection<T>, IMyIterator<T> where T : IComparable<T>
{
    public class Node
    {
        public T Data { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public Node(T data)
        {
            Data = data;
        }
    }

    private Node? _head;

    private T[]? _iteratorArray;
    private int _iteratorIndex;

    private int _count;

    public int Count
    {
        get => _count; 
        set => _count = value;
    }
    public bool Dirty { get; set; }

    // Voldoet aan: void Add(T item) uit de interface
    public void Add(T item)
    {
        if (item == null) return;

        _head = Insert(_head, item);
        _count++;
        Dirty = true;
    }

    public Node Insert(Node? node, T item)
    {
        if(node == null)
            return new Node(item);

        if(item.CompareTo(node.Data) < 0)
            node.Left = Insert(node.Left, item);
        else
            node.Right = Insert(node.Right, item);

        return node;
    }

    public void Update(T item, T newItem)
    {
        if(item == null || newItem == null) return;

        Remove(item);
        Add(newItem);
    }

    public void Remove(T item)
    {
        if (item == null || _head == null) return;

    }
    
    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        if(_head == null) return default;
        Queue<Node> queue = new Queue<Node>();

        queue.Enqueue(_head);

        while(queue.Count > 0)
        {
            var current = queue.Dequeue();

            if(comparer(current.Data, key))
            {
                return current.Data;
            }
            if(current.Left != null)
                queue.Enqueue(current.Left);
            if(current.Right != null)
                queue.Enqueue(current.Right);
        }
        return default;
    }

    private Node? FindNode(Node? node, T item)
    {
        if(node == null) return null;
        int cmp = item.CompareTo(node.Data);
        if(cmp == 0) return node;
        if(cmp < 0)
            return FindNode(node.Left, item);
        return FindNode(node.Right, item);
    }

    public  IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        var filtered = new BinearySearchTree<T>();

        FilterRecursive(_head, predicate, filtered);

        return filtered;
    }

    private void FilterRecursive(Node? node, Func<T, bool> predicate, BinearySearchTree<T> result)
    {
        if(node == null)
            return;
        
        FilterRecursive(node.Left, predicate, result);

        if(predicate(node.Data))
            result.Add(node.Data);

        FilterRecursive(node.Right, predicate, result);
    }
    
    public void Sort(Comparison<T> comparison)
    {
        Dirty = false;
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        return ReduceRecursive(_head, accumulator);
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        return ReduceRecursive(_head, initial, accumulator);
    }

    private R ReduceRecursive<R>(Node? node, R result, Func<R, T, R> accumulator)
    {
        if(node == null)
            return result;

        result = ReduceRecursive(node.Left, result, accumulator);
        result = accumulator(result, node.Data);
        result = ReduceRecursive(node.Right, result, accumulator);
        
        return result;
    }

    private R ReduceRecursive<R>(Node? node, Func<R, T, R> accumulator)
    {
        if(node == null) throw new InvalidOperationException("Lijst is leeg.");

        R result = (R)(object)node.Data;

        result = ReduceRecursive(node.Left, accumulator);
        result = accumulator(result, node.Data);
        result = ReduceRecursive(node.Right, accumulator);

        return result;
    }

    public T[] ToArray()
    {
        T[] array  = new T[_count];
        int i = 0;

        FillArray(_head, array, ref i);
        return array;
    }

    private void FillArray(Node? node, T[] array, ref int index)
    {
        if(node == null)
        {
            return;
        }

        FillArray(node.Left, array, ref index);
        array[index++] = node.Data;
        FillArray(node.Right, array, ref index);
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach(var item in TraverseInOrder(_head))
        {
            yield return item;
        }
    }

    private IEnumerable<T> TraverseInOrder(Node? node)
    {
        if(node == null)
            yield break;
        
        foreach(var item in TraverseInOrder(node.Left))
            yield return item;
        
        yield return node.Data;

        foreach(var item in TraverseInOrder(node.Right))
            yield return item;
    }
    IEnumerator<T> IMyCollection<T>.GetEnumerator()
    {
        return GetEnumerator();
    }
    public IMyIterator<T> GetIterator()
    {
        Reset();
        return this; 
    }
    
    public bool HasNext()
    {
        if(_iteratorArray == null)
            Reset();
        return _iteratorIndex < _iteratorArray.Length;
    }

    public T Next()
    {
        if(!HasNext())
            throw new InvalidOperationException("No more elements");
        
        return _iteratorArray[_iteratorIndex++];
    }

    public void Reset()
    {
        _iteratorArray = ToArray();
        _iteratorIndex = 0;
    }
}