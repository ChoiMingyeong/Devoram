using System.Collections.Concurrent;

namespace DevoramUtility
{
    public class CustomEventHandlerManager<T> where T : notnull
    {
        private ConcurrentDictionary<T, CustomEventHandlerList> _targetEventHandlerLists = new();

        public CustomEventHandlerList? this[T key]
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

        public bool TryAdd(T key)
        {
            if(true == _targetEventHandlerLists.TryGetValue(key, out _))
            {
                return false;
            }

            return _targetEventHandlerLists.TryAdd(key, new());
        }

        public bool TryRemove(T key)
        {
            if (false == _targetEventHandlerLists.TryRemove(key, out var handlerList))
            {
                return false;
            }

            handlerList.Clear();
            return true;
        }
    }
}
