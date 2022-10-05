using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ServiceBusMessages;

namespace ServiceBusPublisher
{
    public interface IServiceBusPublisher
    {
        Task SendMessage(string queueOrTopicName, ServiceBusReceivedMessage message, CancellationToken cancellationToken = default);
        
        Task SendMessage<T>(string queueOrTopicName, ServiceBusMessage<T> message, CancellationToken cancellationToken = default)
            where T : class;

        Task SendMessages<T>(string queueOrTopicName, IEnumerable<ServiceBusMessage<T>> message, CancellationToken cancellationToken = default)
            where T : class;

        Task EnsureTopic(string topicName, CancellationToken cancellationToken = default);

        Task EnsureQueue(string queueName, CancellationToken cancellationToken = default);
    }
}