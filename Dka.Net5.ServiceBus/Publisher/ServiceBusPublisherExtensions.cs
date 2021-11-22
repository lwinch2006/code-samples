using System.Threading;
using Azure.Messaging.ServiceBus;
using ServiceBusMessages;

namespace ServiceBusPublisher
{
    public static class ServiceBusPublisherExtensions
    {
        public static IServiceBusPublisher EnsureTopicEx(this IServiceBusPublisher serviceBusPublisher, string topicName, CancellationToken cancellationToken)
        {
            serviceBusPublisher.EnsureTopic(topicName, cancellationToken).GetAwaiter().GetResult();
            return serviceBusPublisher;
        }
        
        public static IServiceBusPublisher EnsureQueueEx(this IServiceBusPublisher serviceBusPublisher, string queueName, CancellationToken cancellationToken)
        {
            serviceBusPublisher.EnsureQueue(queueName, cancellationToken).GetAwaiter().GetResult();
            return serviceBusPublisher;
        }

        public static ServiceBusMessage EnrichWithMetadata<T>(this ServiceBusMessage serviceBusMessage, ServiceBusMessage<T> message)
            where T : class
        {
            var properties = message.Metadata.GetType().GetProperties();

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(message.Metadata, null);
                serviceBusMessage.ApplicationProperties.Add(property.Name, propertyValue);
            }
            
            return serviceBusMessage;
        }
        
        public static ServiceBusMessage SetCorrelationId<T>(this ServiceBusMessage serviceBusMessage, ServiceBusMessage<T> message)
            where T : class
        {
            if (message.CorrelationId == null)
            {
                return serviceBusMessage;
            }
            
            serviceBusMessage.CorrelationId = message.CorrelationId?.ToString();
            return serviceBusMessage;
        }        
    }
}