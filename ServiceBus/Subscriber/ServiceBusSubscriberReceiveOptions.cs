namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriberReceiveOptions
    {
        public string SessionId { get; set; }
        public bool ConnectToDeadLetterQueue { get; set; }
        public ServiceBusSubscriberReceiveMessageTypes ReceiveMessageType { get; set; }
    }
}