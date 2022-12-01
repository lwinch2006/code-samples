using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ServiceBusSubscriber;

    public class LocalhostServiceBusSubscriber : IServiceBusSubscriber, IServiceBusSubscriberForDeadLetter
    {
        private readonly ILogger<LocalhostServiceBusSubscriber> _logger;
        
        public LocalhostServiceBusSubscriber(ILogger<LocalhostServiceBusSubscriber> logger)
        {
            _logger = logger;
        }

        public Task<T?> ReceiveMessage<T>(string queueOrTopicName, string? subscriptionName = default,
            ServiceBusSubscriberReceiveOptions? options = default, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<object?> ReceiveMessage(string queueOrTopicName, string? subscriptionName = default,
            ServiceBusSubscriberReceiveOptions? options = default, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> ReceiveMessages<T>(string queueOrTopicName, string? subscriptionName = default,
            ServiceBusSubscriberReceiveOptions? options = default, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object> ReceiveMessages(string queueOrTopicName, string? subscriptionName = default,
            ServiceBusSubscriberReceiveOptions? options = default, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task StartReceiveMessages(string queueOrTopicName, string? subscriptionName = default,
            Func<string, string?, object?, Task>? processMessageFunc = default, Func<string, string?, Exception, Task>? processErrorFunc = default,
            ServiceBusSubscriberReceiveOptions? options = default, CancellationToken cancellationToken = default)
        {
            if (options?.ConnectToDeadLetterQueue == true)
            {
                _logger.LogInformation("Start receiving messages from dead letter queue of queue (topic) {QueueOrTopicName} (and subscription {SubscriptionName}) has been registered", queueOrTopicName, subscriptionName);
            }
            else
            {
                _logger.LogInformation("Start receiving messages from queue (topic) {QueueOrTopicName} (and subscription {SubscriptionName}) has been registered", queueOrTopicName, subscriptionName);
            }
            
            return Task.CompletedTask;
        }

        public Task StopReceiveMessages(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Stop receiving messages has been registered");
            return Task.CompletedTask;
        }
        
        public Task EnsureTopicSubscription(string topicName, string subscriptionName, CancellationToken cancellationToken = default,
            string sqlFilterRule = "1=1")
        {
            _logger.LogInformation("Subscription {SubscriptionName} for topic {TopicName} registered to be ensured created", subscriptionName, topicName);
            return Task.CompletedTask;
        }        

        public async Task<long> GetMessageCount(string queueOrTopicName, string subscriptionName = default, bool inDeadLetterQueue = default)
        {
            _logger.LogInformation("Getting message count for queue (topic) {QueueOrTopicName} (and subscription {SubscriptionName}) has been registered", queueOrTopicName, subscriptionName);
            return await Task.FromResult(0);
        }
    }