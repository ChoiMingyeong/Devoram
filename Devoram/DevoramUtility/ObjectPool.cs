using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DevoramUtility
{
    public interface IRecycle
    {
        public abstract void Reset();
    }

    public class ObjectPool<T> where T : IRecycle, new()
    {
        private readonly int DefaultSize = 256;

        private ConcurrentList<T> _objects = new();

        private ConcurrentQueue<T> _waitingObjectQueue = new();

        public ObjectPool(int defaultSize = 0)
        {
            if (defaultSize > 0)
            {
                DefaultSize = defaultSize;
            }

            for (int i = 0; i < DefaultSize; i++)
            {
                CreateObject();
            }
        }

        [return: NotNull]
        private T CreateObject(bool enqueue = true)
        {
            T obj = new T();
            _objects.Add(obj);

            if (enqueue)
            {
                _waitingObjectQueue.Enqueue(obj);
            }

            return obj;
        }


        [return: NotNull]
        public T GetObject()
        {
            if (false == _waitingObjectQueue.TryDequeue(out T? resultObject))
            {
                return CreateObject(false);
            }

            return resultObject;
        }

        public void ReturnObject([NotNull]T obj)
        {
            if(false == _objects.Contains(obj))
            {
                return;
            }

            obj.Reset();
            _waitingObjectQueue.Enqueue(obj);
        }
    }
}
