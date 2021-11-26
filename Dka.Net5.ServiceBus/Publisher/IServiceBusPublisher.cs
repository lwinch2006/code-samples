using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ServiceBusMessages;

namespace ServiceBusPublisher
{
    public interface IServiceBusPublisher
    {
        Task SendMessage(string queueOrTopicName, ServiceBusReceivedMessage message, CancellationToken cancellationToken);
        
        Task SendMessage<T>(string queueOrTopicName, ServiceBusMessage<T> message, CancellationToken cancellationToken)
            where T : class;

        Task SendMessages<T>(string queueOrTopicName, IEnumerable<ServiceBusMessage<T>> message, CancellationToken cancellationToken)
            where T : class;

        Task EnsureTopic(string topicName, CancellationToken cancellationToken);

        Task EnsureQueue(string queueName, CancellationToken cancellationToken);
    }
}