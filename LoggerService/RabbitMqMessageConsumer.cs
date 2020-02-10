using System;
using System.IO;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LoggerService
{
    public class RabbitMqMessageConsumer: IRabbitMqMessageConsumer
    {
        private string exchangeName;
        private string queueName;
        private string routingKey;
        private readonly ConnectionFactory factory;
        private IConnection connection = null;
        private IModel channel = null;
        private EventingBasicConsumer consumer = null;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public RabbitMqMessageConsumer(string hostName, int port, string virtualHost, string exchangeName, string queueName, string routingKey)
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

        public void Start()
        {
            ConnectToServer();
            StartConsuming();
        }

        public void Stop()
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
            Stop();

            var manualResetEventSlim = new ManualResetEventSlim(false);

            while (!manualResetEventSlim.Wait(2000))
            {
                try
                {
                    connection = factory.CreateConnection();
                    connection.ConnectionShutdown += ConnectionShutdownHandler;
                    channel = connection.CreateModel();

                    manualResetEventSlim.Set();
                }
                catch
                {
                }
            }
        }

        private void ConnectionShutdownHandler(object sender, ShutdownEventArgs e)
        {
            ConnectToServer();
        }

        private void StartConsuming()
        {
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReceivedHandler;

            channel.BasicConsume(queueName, false, consumer);
        }

        private void ReceivedHandler(object sender, BasicDeliverEventArgs e)
        {
            if (MessageReceived != null)
            {
                var args = new MessageReceivedEventArgs
                {
                    Message = e.Body,
                    Ack = false
                };
                MessageReceived(this, args);
                if (args.Ack)
                {
                    channel.BasicAck(e.DeliveryTag, false);
                    return;
                }
            }
            channel.BasicNack(e.DeliveryTag, false, true);
        }
    }
}
