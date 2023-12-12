using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DevoramUtility
{
    public class CustomEventHandlerManager<T> where T : notnull
    {
        static private ObjectPool<CustomEventHandlerList> _handlerPool = new();
        private ConcurrentDictionary<T, CustomEventHandlerList> _targetEventHandlerLists = new();

        private CustomEventHandlerList? this[T key]
        {
            get
            {
                if (false == _targetEventHandlerLists.TryGetValue(key, out var handlerList))
                {
                    return null;
                }

                return handlerList;
            }
        }

        public void Add(T key, int eventType, [NotNull] EventHandler handler)
        {
            if(false == _targetEventHandlerLists.TryGetValue(key, out var eventHandlerList))
            {
                eventHandlerList = new();
                _targetEventHandlerLists.TryAdd(key, eventHandlerList);
            }

            eventHandlerList.Add(eventType, handler);
        }

        public void Remove(T key, int eventType, [NotNull] EventHandler handler)
        {
            if (false == _targetEventHandlerLists.TryGetValue(key, out var eventHandlerList))
            {
                return;
            }

            eventHandlerList.Remove(eventType, handler);
        }

        public bool TryRemove(T key)
        {
            if (false == _targetEventHandlerLists.TryRemove(key, out var handlerList))
            {
                return false;
            }

            _handlerPool.ReturnObject(handlerList);
            return true;
        }

        public void NotifyEvent(T key, int eventType, [NotNull] CustomEventArgs args)
        {
            this[key]?.NotifyEvent(eventType, args);
        }
    }
}
