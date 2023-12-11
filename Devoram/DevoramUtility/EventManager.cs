using System.Collections.Concurrent;
using System.ComponentModel;

namespace DevoramUtility
{
    public class EventManager<T> where T : notnull
    {
        private ConcurrentDictionary<EventKey, EventHandlerList> _eventHandlerLists = new();
        private ConcurrentDictionary<T, EventKey> _eventKeyMapping = new();
        private ObjectPool<EventKey> _eventKeyPool = new ObjectPool<EventKey>();

        public EventManager()
        {

        }
    }
}
