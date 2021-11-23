using System;

namespace ServiceBusMessages
{
    public class ServiceBusMessage<T> 
        where T : class
    {
        public Metadata Metadata { get; set; }
        public T Payload { get; set; }
        public Guid? CorrelationId { get; set; }  
        public string SessionId { get; set; }
        public string ReplyToSessionId { get; set; }        
    }
}