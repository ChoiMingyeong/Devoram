using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace DevoramUtility
{
    public class CustomEventHandlerList : IRecycle
    {
        private static ObjectPool<EventKey> _eventKeyObjectPool = new ObjectPool<EventKey>();

        private ConcurrentDictionary<int, EventKey> _eventKeyMapping = new();
        private EventHandlerList _eventHandlerList = new EventHandlerList();

        public EventHandler? this[int eventType]
        {
            get
            {
                if (false == _eventKeyMapping.TryGetValue(eventType, out var eventKey) ||
                    _eventHandlerList[eventKey] is not EventHandler eventHandler)
                {
                    return null;
                }

                return eventHandler;
            }
        }

        public bool TryGetValue(int eventType, [NotNullWhen(true)] out EventHandler? value)
        {
            if (this[eventType] is not EventHandler eventHandler)
            {
                value = null;
                return false;
            }

            value  = eventHandler;
            return true;

        }

        public void Add(int eventType, EventHandler eventHandler)
        {
            if (false == _eventKeyMapping.TryGetValue(eventType, out var eventKey))
            {
                eventKey = _eventKeyObjectPool.GetObject();
                _eventKeyMapping.TryAdd(eventType, eventKey);
            }

            _eventHandlerList.AddHandler(eventKey, eventHandler);
        }

        public void Remove(int eventType, EventHandler eventHandler)
        {
            if (false == _eventKeyMapping.TryGetValue(eventType, out var eventKey))
            {
                return;
            }

            _eventHandlerList.RemoveHandler(eventKey, eventHandler);
        }

        public void NotifyEvent(int eventType, CustomEventArgs args)
        {
            this[eventType]?.Invoke(eventType, args);
        }

        public void Reset()
        {
            _eventHandlerList.Dispose();

            foreach (var eventKey in _eventKeyMapping.Values)
            {
                _eventKeyObjectPool.ReturnObject(eventKey);
            }

            _eventKeyMapping.Clear();
        }
    }
}
