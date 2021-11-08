namespace ServiceBusMessages
{
    public class ServiceBusMessage<T> 
        where T : class
    {
        public Metadata Metadata { get; set; }
        public T Payload { get; set; }
    }
}