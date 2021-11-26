using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusMessages;

namespace ServiceBusPublisher
{
    public class ServiceBusPublisher : IServiceBusPublisher
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _administrationClient;
        private readonly ILogger<ServiceBusPublisher> _logger;

        public ServiceBusPublisher(
            ServiceBusClient client,
            ServiceBusAdministrationClient administrationClient,
            ILogger<ServiceBusPublisher> logger)
        {
            _client = client;
            _administrationClient = administrationClient;
            _logger = logger;
        }

        public async Task SendMessage(string queueOrTopicName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            ServiceBusSender publisher = null;
            
            try
            {
                publisher = _client.CreateSender(queueOrTopicName);
                var serviceBusMessage = new ServiceBusMessage(message);
                await publisher.SendMessageAsync(serviceBusMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus message sending error");
                throw new ServiceBusPublisherOperationException("Service Bus message sending error", ex);
            }
            finally
            {
                if (publisher != null)
                {
                    await publisher.DisposeAsync();
                }
            }
        }
        
        public async Task SendMessage<T>(string queueOrTopicName, ServiceBusMessage<T> message, CancellationToken cancellationToken) 
            where T : class
        {
            ServiceBusSender publisher = null;
            
            try
            {
                publisher = _client.CreateSender(queueOrTopicName);
                var messageAsJson = JsonConvert.SerializeObject(message);
                var serviceBusMessage = new ServiceBusMessage(messageAsJson)
                    .EnrichWithMetadata(message)
                    .SetCorrelationId(message)
                    .SetSessionId(message);
                
                await publisher.SendMessageAsync(serviceBusMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus message sending error");
                throw new ServiceBusPublisherOperationException("Service Bus message sending error", ex);
            }
            finally
            {
                if (publisher != null)
                {
                    await publisher.DisposeAsync();
                }
            }
        }
        
        public async Task SendMessages<T>(string queueOrTopicName, IEnumerable<ServiceBusMessage<T>> messages, CancellationToken cancellationToken) 
            where T : class
        {
            var publisher = _client.CreateSender(queueOrTopicName);
            using var serviceBusMessages = await publisher.CreateMessageBatchAsync(cancellationToken);

            foreach (var message in messages)
            {
                var messageAsJson = JsonConvert.SerializeObject(message);
                var serviceBusMessage = new ServiceBusMessage(messageAsJson)
                    .EnrichWithMetadata(message)
                    .SetCorrelationId(message)
                    .SetSessionId(message);
                
                if (!serviceBusMessages.TryAddMessage(serviceBusMessage))
                {
                    throw new ServiceBusPublisherOperationException($"The message {message.Metadata.EventType}.{message.Metadata.EventName} is too large to fit in the batch.");
                }
            }
            
            await publisher.SendMessagesAsync(serviceBusMessages, cancellationToken);
            await publisher.DisposeAsync();
        }

        public async Task EnsureTopic(string topicName, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _administrationClient.TopicExistsAsync(topicName, cancellationToken))
                {
                    var options = new CreateTopicOptions(topicName)
                    {
                        Status = EntityStatus.Active,
                        MaxSizeInMegabytes = 1024,
                        DefaultMessageTimeToLive = TimeSpan.FromDays(10000)
                    };

                    await _administrationClient.CreateTopicAsync(options, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus ensure topic operation failure");
                throw new ServiceBusPublisherOperationException("Service Bus ensure topic operation failure", ex);
            }
        }

        public async Task EnsureQueue(string queueName, CancellationToken cancellationToken)
        {
            if (!await _administrationClient.QueueExistsAsync(queueName, cancellationToken))
            {
                var options = new CreateQueueOptions(queueName)
                {
                    Status = EntityStatus.Active,
                    MaxSizeInMegabytes = 1024,
                    DefaultMessageTimeToLive = TimeSpan.FromDays(10000),
                    RequiresSession = queueName.Contains("response", StringComparison.OrdinalIgnoreCase)
                };

                await _administrationClient.CreateQueueAsync(options, cancellationToken);
            }            
        }
    }
}