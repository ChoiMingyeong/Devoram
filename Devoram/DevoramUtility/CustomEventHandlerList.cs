using System.Collections.Concurrent;
using System.ComponentModel;

namespace DevoramUtility
{
    public class CustomEventHandlerList
    {
        private static ObjectPool<EventKey> _eventKeyObjectPool = new ObjectPool<EventKey>();

        private ConcurrentDictionary<byte, EventKey> _eventKeyMapping = new();
        private EventHandlerList _eventHandlerList = new EventHandlerList();

        private EventHandler? this[byte index]
        {
            get
            {
                if (false == _eventKeyMapping.TryGetValue(index, out var key))
                {
                    return null;
                }

                if (_eventHandlerList[key] is not EventHandler eventHandler)
                {
                    return null;
                }

                return eventHandler;
            }
        }

        public void AddHandler(byte index, EventHandler? eventHandler)
        {
            if (false == _eventKeyMapping.TryGetValue(index, out var key))
            {
                key = _eventKeyObjectPool.GetObject();
                _eventKeyMapping.TryAdd(index, key);
            }

            _eventHandlerList.AddHandler(key, eventHandler);
        }

        public void RemoveHandler(byte index, EventHandler? eventHandler)
        {
            if (false == _eventKeyMapping.TryGetValue(index, out var key))
            {
                return;
            }

            _eventKeyObjectPool.ReturnObject(key);
            _eventHandlerList.RemoveHandler(key, eventHandler);
        }

        public void NotifyRaisedEvent(byte index, EventArgs e)
        {
            if (false == _eventKeyMapping.TryGetValue(index, out var key))
            {
                return;
            }

            this[index]?.Invoke(this, e);
        }
    }
}
