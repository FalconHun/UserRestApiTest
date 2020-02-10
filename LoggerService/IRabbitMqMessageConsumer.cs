using System;

namespace LoggerService
{
    public interface IRabbitMqMessageConsumer
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        void Start();
        void Stop();
    }
}
