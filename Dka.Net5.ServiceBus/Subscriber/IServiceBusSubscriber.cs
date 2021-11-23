using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusSubscriber
{
    public interface IServiceBusSubscriber
    {
        Task<T> ReceiveMessage<T>(
            string queueOrTopicName, 
            string subscriptionName = default, 
            ServiceBusSubscriberReceiveOptions options = default, 
            CancellationToken cancellationToken = default)
            where T : class;

        Task<object> ReceiveMessage(
            string queueOrTopicName, 
            string subscriptionName = default, 
            ServiceBusSubscriberReceiveOptions options = default, 
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<T> ReceiveMessages<T>(
            string queueOrTopicName, 
            string subscriptionName = default, 
            ServiceBusSubscriberReceiveOptions options = default, 
            CancellationToken cancellationToken = default)
            where T : class;
        
        IAsyncEnumerable<object> ReceiveMessages(
            string queueOrTopicName, 
            string subscriptionName = default, 
            ServiceBusSubscriberReceiveOptions options = default, 
            CancellationToken cancellationToken = default);

        Task StartReceiveMessages(
            string queueOrTopicName,
            string subscriptionName = default,
            Func<object, Task> processMessageFunc = default,
            Func<Exception, Task> processErrorFunc = default,
            ServiceBusSubscriberReceiveOptions options = default,
            CancellationToken cancellationToken = default);

        Task StopReceiveMessagesFromTopicSubscription(CancellationToken cancellationToken);
        
        Task EnsureTopicSubscription(
            string topicName, 
            string subscriptionName, 
            CancellationToken cancellationToken, 
            string sqlFilterRule = "1=1");
    }
}