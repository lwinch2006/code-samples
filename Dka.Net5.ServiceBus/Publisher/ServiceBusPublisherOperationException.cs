using System;

namespace ServiceBusPublisher
{
    public class ServiceBusPublisherOperationException : Exception
    {
        public ServiceBusPublisherOperationException(string? message)
        : base(message)
        {
        }
        
        public ServiceBusPublisherOperationException(string? message, Exception? ex)
            : base(message, ex)
        {
        }        
    }
}