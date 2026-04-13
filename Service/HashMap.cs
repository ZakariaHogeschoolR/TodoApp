public class HashMap<K, T>: IMyCollection<T>, IMyIterator<T> where T : IHasId<K>
{ 
    private LinkedList<T>[] _buckets;

    private int _numBuckets;

    private int _count;

    public int Count
    {
        get => _count; 
        set => _count = value;
    }
    public bool Dirty { get; set; }
    private int _iteratorIndex;

    public HashMap(int capacity = 10)
    {
        _numBuckets = capacity;
        _buckets = new LinkedList<T>[capacity];
    }

    private int GetBucketIndex(K key)
    {
        if(key == null) return 0;
        return Math.Abs(key.GetHashCode()) % _numBuckets;
    }

    // Voldoet aan: void Add(T item) uit de interface
    public void Add(T item)
    {
        if (item == null) return;

        int index = GetBucketIndex(item.Id);

        if(_buckets[index] == null)
        {
            _buckets[index] = new LinkedList<T>();
        }

        _buckets[index].Add(item);

        _count++;
        Dirty = true;
    }

    public void Update(T item, T newItem)
    {
        if(item == null || newItem == null) return;

        int index = GetBucketIndex(newItem.Id);
        var bucketList = _buckets[index];

        if(bucketList == null) return;

        bucketList.Update(item, newItem);

        Dirty = true;
    }

    public void Remove(T item)
    {
        if (item == null) return;
        int index = GetBucketIndex(item.Id);
        var bucketList = _buckets[index];

        if(bucketList == null) return;

        int oldCount = bucketList.Count;
        bucketList.Remove(item);
        if(bucketList.Count < oldCount)
        {
            _count--;
            Dirty = true;
        }
    }
    
    public T? FindBy<TK>(TK key, Func<T, TK, bool> comparer)
    {
        if (key is K specificKey)
        {
            int index = GetBucketIndex(specificKey);
            var bucketList = _buckets[index];

            if (bucketList == null) return default;

            return bucketList.FindBy(key, comparer);
        }
        return default;
    }

    public  IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        var result = new LinkedList<T>();

        foreach(var bucket in _buckets)
        {
            if(bucket != null)
            {
                var filteredBucket = bucket.Filter(predicate);

                foreach(var item in filteredBucket)
                {
                    result.Add(item);
                }
            }
        }
        return result;
    }
    
    public void Sort(Comparison<T> comparison)
    {
        foreach(var bucket in _buckets)
        {
            bucket?.Sort(comparison);
        }
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        if(_count == 0) return default(R);
        R result = default!;
        bool assignedInitial = false;
        foreach(var bucket in _buckets)
        {
            if(bucket != null && bucket.Count > 0)
            {
                if(!assignedInitial)
                {
                    result = bucket.Reduce(accumulator);
                    assignedInitial = true;
                }
                else
                {
                    result = bucket.Reduce(result, accumulator);
                }
            }
        }
        return result;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R current = initial;

        foreach(var bucket in _buckets)
        {
            if(bucket != null)
            {
                current = bucket.Reduce(current, accumulator);
            }
        }
        return current;
    }

    public T[] ToArray()
    {
        T[] array  = new T[_count];
        int i = 0;

        foreach(var bucket in _buckets)
        {
            if(bucket != null)
            {
                foreach(var item in bucket)
                {
                    array[i] = item;
                    i++;
                }
            }
        }
        return array;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach(var bucket in _buckets)
        {
            if(bucket != null)
            {
                foreach(var item in bucket)
                {
                    yield return item;
                }
            }
        }
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
    
    public bool HasNext() => _iteratorIndex + 1 < _count;

    public T Next()
    {
        T[] _iteratorArray = ToArray();
        if (_iteratorArray == null || Dirty) _iteratorArray = ToArray();
        return _iteratorArray[++_iteratorIndex];
    }

    public void Reset()
    {
        T[] _iteratorArray = ToArray();
        _iteratorIndex = -1;
    }
}