using System;
using System.IO;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace UserService
{
    public class RabbitMqMessagePublisher : IRabbitMqMessagePublisher, IDisposable
    {
        private string exchangeName;
        private string queueName;
        private string routingKey;
        private readonly ConnectionFactory factory;
        private IConnection connection = null;
        private IModel channel = null;

        public RabbitMqMessagePublisher(IConnectionParamProvider connParams) :
            this(connParams.HostName, connParams.Port, connParams.VirtualHost, connParams.ExchangeName, connParams.QueueName, connParams.RoutingKey)
        {

        }

        private  RabbitMqMessagePublisher(string hostName, int port, string virtualHost, string exchangeName, string queueName, string routingKey)
        {
            this.exchangeName = exchangeName;
            this.queueName = queueName;
            this.routingKey = routingKey;

            factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                VirtualHost = virtualHost,
                RequestedHeartbeat = 20
            };
        }

        public void PublishMessage(string message)
        {
            ConnectToServer();
               
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchangeName, routingKey, null, body);
        }

        private void Stop()
        {
            try
            {
                if (channel != null && channel.IsOpen)
                {
                    channel.Close();
                    channel = null;
                }

                if (connection != null && connection.IsOpen)
                {
                    connection.Close();
                    connection = null;
                }
            }
            catch (IOException)
            {
            }
        }

        private void ConnectToServer()
        {
            if (IsConnectionOpen() && IsChannelOpen())
            {
                return;
            }

            var manualResetEventSlim = new ManualResetEventSlim(false);

            int tryCount = 3;

            while (!manualResetEventSlim.Wait(2000))
            {
                try
                {
                    if (!IsConnectionOpen())
                    {
                        connection = factory.CreateConnection();
                        connection.ConnectionShutdown += ConnectionShutdownHandler;
                        channel = connection.CreateModel();
                    }
                    else if (!IsChannelOpen())
                    {
                        channel = connection.CreateModel();
                    }

                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                    channel.QueueDeclare(queueName, false, false, false, null);
                    channel.QueueBind(queueName, exchangeName, routingKey, null);

                    manualResetEventSlim.Set();
                }
                catch
                {
                    if (tryCount == 0)
                        throw;
                    tryCount--;
                }
            }
        }

        private void ConnectionShutdownHandler(object sender, ShutdownEventArgs e)
        {
            Stop();
            ConnectToServer();
        }

        private bool IsConnectionOpen()
        {
            return connection != null && connection.IsOpen;
        }
        private bool IsChannelOpen()
        {
            return channel != null && channel.IsOpen;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
