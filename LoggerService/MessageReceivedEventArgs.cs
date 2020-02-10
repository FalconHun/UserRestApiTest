
namespace LoggerService
{
    public class MessageReceivedEventArgs
    {
        public byte[] Message { get; set; }
        public bool Ack { get; set; }
    }
}
