namespace UserService
{
    public interface IRabbitMqMessagePublisher
    {
        void PublishMessage(string message);
    }
}
