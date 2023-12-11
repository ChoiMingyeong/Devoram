using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DevoramUtility
{
    public class MessageQueue<T> where T : IMessage
    {
        private ConcurrentQueue<T> _messageQueue = new();

        public void Enqueue([NotNull] T message)
        {
            _messageQueue.Enqueue(message);
        }

        // EventHandler delegate 형식에 맞추기 위한 매개변수 형식(의미 x)
        public void Execute(object? sender, EventArgs args)
        {
            while (true == _messageQueue.TryDequeue(out var message))
            {
                message.Execute();
            }
        }
    }
}
