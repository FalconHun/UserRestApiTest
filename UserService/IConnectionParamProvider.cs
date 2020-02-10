namespace UserService
{
    public interface IConnectionParamProvider
    {
        string HostName { get; set; }
        int Port { get; set; }
        string VirtualHost { get; set; }
        string ExchangeName { get; set; }
        string QueueName { get; set; }
        string RoutingKey { get; set; }
    }
}
