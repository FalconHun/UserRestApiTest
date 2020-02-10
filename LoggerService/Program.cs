using System;
using System.Text;
using System.Threading;

namespace LoggerService
{
    class Program
    {
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            IEnvironmentWrapper env = new EnvironmentWrapper();

            var hostName = env.GetValue("hostname", "localhost");
            if (!int.TryParse(env.GetValue("port", ""), out int port))
            {
                port = 1;
            }
            var virtualHost = env.GetValue("virtualhost", "/");
            var exchangeName = env.GetValue("exchangename", "DefaultExchange");
            var queueName = env.GetValue("queuename", "DefaultQueue"); ;
            var routingKey = env.GetValue("RoutingKey", "");

            var consumer = new RabbitMqMessageConsumer(hostName, port, virtualHost, exchangeName, queueName, routingKey);

            consumer.MessageReceived += Consumer_MessageReceived;

            try
            {

                consumer.Start();
                Console.WriteLine("UserCreated listener started.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"RabbitMq consumer error: {exception.Message}");
            }

            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            _closing.WaitOne();

            Console.WriteLine("UserCreated listener is stopping...");

            consumer.Stop();

            Console.WriteLine("UserCreated listener is stopped");
        }

        private static void Consumer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                string message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
                Console.WriteLine($"User was created with id: {message}");
                e.Ack = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                e.Ack = false;
            }
        }

        static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            _closing.Set();
        }
    }
}
