using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace DevoramUtility
{
    public class CustomEventHandlerList
    {
        private static ObjectPool<EventKey> _eventKeyObjectPool = new ObjectPool<EventKey>();

        private ConcurrentDictionary<int, EventKey> _eventKeyMapping = new();
        private EventHandlerList _eventHandlerList = new EventHandlerList();

        public EventHandler? this[int key]
        {
            get
            {
                if (false == _eventKeyMapping.TryGetValue(key, out var eventKey))
                {
                    return null;
                }

                if (_eventHandlerList[eventKey] is not EventHandler eventHandler)
                {
                    return null;
                }

                return eventHandler;
            }

            set
            {
                var eventHandler = this[key];

                if (value is null)
                {
                    if (eventHandler is not null)
                    {
                        TryRemove(key);
                    }

                    return;
                }

                if (eventHandler is null)
                {
                    TryAdd(key, value);
                    return;
                }

                if (true == _eventKeyMapping.TryGetValue(key, out var eventkey))
                {
                    _eventHandlerList.RemoveHandler(eventkey, eventHandler);
                    _eventHandlerList.AddHandler(eventkey, value);
                }
            }
        }

        public bool TryGetValue(int key, [NotNullWhen(true)] out EventHandler? value)
        {
            if (this[key] is not EventHandler eventHandler)
            {
                value = null;
                return false;
            }

            value  = eventHandler;
            return true;

        }

        public bool TryAdd(int key, EventHandler eventHandler)
        {
            if (true == _eventKeyMapping.TryGetValue(key, out _))
            {
                return false;
            }

            var eventKey = _eventKeyObjectPool.GetObject();
            _eventKeyMapping.TryAdd(key, eventKey);
            _eventHandlerList.AddHandler(eventKey, eventHandler);
            return true;
        }

        public bool TryRemove(int key)
        {
            if (false == _eventKeyMapping.TryRemove(key, out var eventKey))
            {
                return false;
            }

            _eventHandlerList.RemoveHandler(eventKey, _eventHandlerList[eventKey]);
            _eventKeyObjectPool.ReturnObject(eventKey);
            return true;
        }

        public void Clear()
        {
            _eventHandlerList.Dispose();

            foreach (var eventKey in _eventKeyMapping.Values)
            {
                _eventKeyObjectPool.ReturnObject(eventKey);
            }

            _eventKeyMapping.Clear();
        }

        public void NotifyRaisedEvent(int key, EventArgs e)
        {
            this[key]?.Invoke(this, e);
        }
    }
}
