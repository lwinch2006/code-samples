using System;

namespace ServiceBusMessages
{
    public class ServiceBusMessage<T> 
        where T : class
    {
        public Guid? CorrelationId { get; set; }        
        public Metadata Metadata { get; set; }
        public T Payload { get; set; }
    }
}