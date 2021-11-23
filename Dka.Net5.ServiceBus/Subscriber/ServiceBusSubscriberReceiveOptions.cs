namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriberReceiveOptions
    {
        public string SessionId { get; set; }
        public bool ReturnFullMessage { get; set; }
    }
}