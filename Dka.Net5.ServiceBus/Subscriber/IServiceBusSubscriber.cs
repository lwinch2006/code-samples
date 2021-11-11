using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusSubscriber
{
    public interface IServiceBusSubscriber
    {
        Task<T> ReceiveMessageFromQueue<T>(string queueName, CancellationToken cancellationToken)
            where T : class;
        
        IAsyncEnumerable<T> ReceiveMessagesFromQueue<T>(string queueName, CancellationToken cancellationToken)
            where T : class;

        Task<object> ReceiveMessageFromQueue(string queueName, CancellationToken cancellationToken);

        IAsyncEnumerable<object> ReceiveMessagesFromQueue(string queueName, CancellationToken cancellationToken);
        
        Task<T> ReceiveMessageFromTopicSubscription<T>(string topic, string subscription, CancellationToken cancellationToken)
            where T : class;

        Task<object> ReceiveMessageFromTopicSubscription(string topic, string subscription, CancellationToken cancellationToken);        
        
        IAsyncEnumerable<T> ReceiveMessagesFromTopicSubscription<T>(string topic, string subscription, CancellationToken cancellationToken)
            where T : class;

        IAsyncEnumerable<object> ReceiveMessagesFromTopicSubscription(string topic, string subscription, CancellationToken cancellationToken);
        
        Task EnsureTopicSubscription(string topicName, string subscriptionName, CancellationToken cancellationToken, string sqlFilterRule = "1=1");

        Task StartReceiveMessagesFromTopicSubscription(
            string topic,
            string subscription,
            Func<object, Task> processMessageFunc,
            Func<Exception, Task> processErrorFunc,
            CancellationToken cancellationToken);

        Task StopReceiveMessagesFromTopicSubscription(CancellationToken cancellationToken);
    }
}