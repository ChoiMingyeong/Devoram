namespace DevoramUtility
{
    public class CustomEventArgs : EventArgs, IRecycle
    {
        public int EventTypeKey { get; private set; }
        public object?[]? Values { get; private set; }

        public void Set(int eventTypeKey, params object?[]? values)
        {
            EventTypeKey = eventTypeKey;
            Values = values;
        }

        public void Reset()
        {
            EventTypeKey = 0;
            Values = null;
        }
    }
}
