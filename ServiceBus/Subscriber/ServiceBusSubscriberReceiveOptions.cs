namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriberReceiveOptions
    {
        public string? SessionId { get; init; }
        public bool ConnectToDeadLetterQueue { get; init; }
        public ServiceBusSubscriberReceiveMessageTypes ReceiveMessageType { get; init; }
    }
}