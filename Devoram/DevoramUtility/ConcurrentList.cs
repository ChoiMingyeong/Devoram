namespace DevoramUtility
{
    public class ConcurrentList<T>
    {
        ~ConcurrentList()
        {
            _lockSlim?.Dispose();
        }

        public T? this[int index]
        {
            get
            {
                if (index < 0 || _values.Count <= index)
                {
                    return default;
                }

                return (T?)ReadExecute(() => _values[index]);
            }

            set
            {
                if (index < 0 || _values.Count <= index)
                {
                    return;
                }

                WriteExecute(() => _values[index] = value);
            }
        }

        public int Count => (int)ReadExecute(() => _values.Count)!;

        public bool Contains(T obj) => (bool)ReadExecute(() => _values.Contains(obj))!;

        public void Add(T obj, int timeout = 0) => WriteExecute(() => _values.Add(obj), timeout);

        public void Remove(T obj, int timeout = 0) => WriteExecute(() => _values.Remove(obj), timeout);

        public void Clear(int timeout = 0) => WriteExecute(() => _values.Clear(), timeout);

        private void WriteExecute(Delegate @delegate, int timeout = 0, params object?[]? args)
        {
            if (timeout <= 0)
            {
                _lockSlim.EnterWriteLock();
            }
            else
            {
                _lockSlim.TryEnterWriteLock(timeout);
            }

            try
            {
                @delegate.DynamicInvoke(args);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        private object? ReadExecute(Delegate @delegate, int timeout = 0, params object?[]? args)
        {
            if (timeout <= 0)
            {
                _lockSlim.EnterReadLock();
            }
            else
            {
                _lockSlim.TryEnterReadLock(timeout);
            }

            try
            {
                return @delegate.DynamicInvoke(args);
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        private ReaderWriterLockSlim _lockSlim = new();

        private List<T?> _values = new();
    }
}
