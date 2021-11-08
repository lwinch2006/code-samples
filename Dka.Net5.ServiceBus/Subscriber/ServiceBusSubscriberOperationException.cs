using System;

namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriberOperationException : Exception
    {
        public ServiceBusSubscriberOperationException(string? message)
            : base(message)
        {}
        
        public ServiceBusSubscriberOperationException(string? message, Exception? innerException)
            : base(message, innerException)
        {}
    }
}