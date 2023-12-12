namespace DevoramUtility
{
    public class CustomEventArgs : EventArgs, IRecycle
    {
        public object?[]? Values { get; private set; }

        public void Set(params object?[]? values)
        {
            Values = values;
        }

        public void Reset()
        {
            Values = null;
        }
    }
}
