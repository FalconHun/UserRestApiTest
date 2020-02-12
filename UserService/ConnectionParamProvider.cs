namespace UserService
{
    public class ConnectionParamProvider : IConnectionParamProvider
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }

        public ConnectionParamProvider(IEnvironmentWrapper env)
        {
            HostName = env.GetValue("hostname", "localhost");
            if (!int.TryParse(env.GetValue("port", ""), out int port))
            {
                port = 1;
            }
            Port = port;
            VirtualHost = env.GetValue("virtualhost", "/");
            ExchangeName = env.GetValue("exchangename", "DefaultExchange");
            QueueName = env.GetValue("queuename", "DefaultQueue"); ;
            RoutingKey = env.GetValue("RoutingKey", "");
        }
    }
}
