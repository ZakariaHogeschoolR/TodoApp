
public class LinkedList<T> : IMyCollection<T>, IMyIterator<T>
{
    private Node? _currentIteratorNode;
    private Node? _current;
    // De Node moet ook T gebruiken, niet hardcoded TaskItem
    public class Node
    {
        public T Data { get; set; }
        public Node? Next { get; set; }
        public Node? Prev { get; set; } // Handig voor verwijderen

        public Node(T data)
        {
            Data = data;
        }
    }

    private Node? _head;
    private Node? _tail;
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

        var newNode = new Node(item);

        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            // Direct plakken aan de tail (geen while-loop nodig!)
            _tail!.Next = newNode;
            newNode.Prev = _tail;
            _tail = newNode;
        }

        _count++;
        Dirty = true;
    }

    public void Update(T item, T newItem)
    {
        if(item == null || newItem == null) return;

        Node current = _head;

        while (current != null)
        {
            if(current.Data != null && current.Data.Equals(item))
            {
                current.Data = newItem;
                Dirty = true;
                return;
            }
            current = current.Next;
        }
    }

    public void Remove(T item)
    {
        if (item == null || _head == null) return;
        Node? current = _head;
        while (current != null)
        {
            if (current.Data != null && current.Data.Equals(item))
            {
                if (current.Prev != null)
                {
                    current.Prev.Next = current.Next;
                }
                else
                {
                    _head = current.Next;
                }
                if (current.Next != null)
                {
                    current.Next.Prev = current.Prev;
                }
                else
                {
                    _tail = current.Prev;
                }
                _count--;
                Dirty = true;
                return;
            }
            current = current.Next;
        }
    }
    
    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        Node? current = _head;
        while(current != null)
        {
            if(comparer(current.Data, key))
            {
                return current.Data;
            }
            current = current.Next;
        }
        return default;
    }

    public  IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        var filteredList = new LinkedList<T>();
        Node? current = _head;

        while(current != null )
        {
            if(predicate(current.Data))
            {
                filteredList.Add(current.Data);
            }
            current = current.Next;
        }
        return filteredList;
    }
    
    public void Sort(Comparison<T> comparison)
    {
        if(_head == null || _head.Next == null) return;

        bool swapped;
        do
        {
            swapped = false;
            Node current = _head;

            while (current.Next != null)
            {
                if(comparison(current.Data, current.Next.Data) > 0)
                {
                    T Temp = current.Data;
                    current.Data = current.Next.Data;
                    current.Next.Data = Temp;
                    swapped = true;
                }
                current = current.Next;
            }
        } while(swapped);
        Dirty = false;
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        if(_head == null) throw new InvalidOperationException("Lijst is leeg.");

        R result = (R)(object)_head.Data;
        Node current = _head.Next;

        while(current != null)
        {
            result = accumulator(result, current.Data);
            current = current.Next;
        }
        return result;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;
        Node current = _head;

        while (current != null)
        {
            result = accumulator(result, current.Data);
            current = current.Next;
        }
        return result;
    }

    public T[] ToArray()
    {
        T[] array  = new T[_count];
        Node current = _head;
        int i = 0;

        while(current != null)
        {
            array[i] = current.Data;
            current = current.Next;
            i++; 
        }
        return array;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node? current = _head;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator<T> IMyCollection<T>.GetEnumerator()
    {
        return GetEnumerator();
    }
    public IMyIterator<T> GetIterator()
    {
        return this; 
    }
    
    public bool HasNext() => _current == null ? _head != null : _current.Next != null;

    public T Next()
    {
        if (_current == null) _current = _head;
        else _current = _current.Next;
        return _current.Data;
    }

    public void Reset() => _current = null;
}