using System.Collections.Concurrent;

namespace DevoramUtility
{
    public class CustomEventHandlerManager<T> where T : notnull
    {
        private ConcurrentDictionary<T, CustomEventHandlerList> _targetEventHandlerMapping = new();

        public void AddEventDelegate(T target, byte index, EventHandler? eventHandler)
        {
            if (false == _targetEventHandlerMapping.TryGetValue(target, out var handlers))
            {
                handlers = new CustomEventHandlerList();
                _targetEventHandlerMapping.TryAdd(target, handlers);
            }

            handlers.AddHandler(index, eventHandler);
        }

        public void RemoveEventDelegate(T target, byte index, EventHandler? eventHandler)
        {
            if (false == _targetEventHandlerMapping.TryGetValue(target, out var handlers))
            {
                return;
            }

            handlers.RemoveHandler(index, eventHandler);
        }

        public void NotifyRaisedEvent(T target, byte index, EventArgs e)
        {
            if (false == _targetEventHandlerMapping.TryGetValue(target, out var handlers))
            {
                return;
            }

            handlers.NotifyRaisedEvent(index, e);
        }
    }
}
