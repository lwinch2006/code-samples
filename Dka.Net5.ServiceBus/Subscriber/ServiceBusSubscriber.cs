using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Newtonsoft.Json;
using ServiceBusMessages;

namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriber : IServiceBusSubscriber
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _administrationClient;
        private readonly ServiceBusSubscriberConfiguration _serviceBusSubscriberConfiguration;

        public ServiceBusSubscriber(
            ServiceBusClient client,
            ServiceBusAdministrationClient administrationClient,
            ServiceBusSubscriberConfiguration serviceBusSubscriberConfiguration)
        {
            _client = client;
            _administrationClient = administrationClient;
            _serviceBusSubscriberConfiguration = serviceBusSubscriberConfiguration;
        }        
        
        public async Task<T> ReceiveMessageFromQueue<T>(string queueName, CancellationToken cancellationToken) 
            where T : class
        {
            await using var receiver = _client.CreateReceiver(queueName);
            var serviceBusMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);

            if (serviceBusMessage == null)
            {
                return null;
            }

            return await GetPayload<T>(receiver, serviceBusMessage, cancellationToken);
        }
        
        public async IAsyncEnumerable<T> ReceiveMessagesFromQueue<T>(string queueName, CancellationToken cancellationToken) 
            where T : class
        {
            await using var receiver = _client.CreateReceiver(queueName);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                yield return await GetPayload<T>(receiver, serviceBusMessage, cancellationToken);
            }
        }

        public async Task<object> ReceiveMessageFromQueue(string queueName, CancellationToken cancellationToken)
        {
            await using var receiver = _client.CreateReceiver(queueName);
            var serviceBusMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);

            if (serviceBusMessage == null)
            {
                return null;
            }
            
            return await GetPayload(receiver, serviceBusMessage, cancellationToken);
        }

        public async IAsyncEnumerable<object> ReceiveMessagesFromQueue(string queueName, CancellationToken cancellationToken)
        {
            await using var receiver = _client.CreateReceiver(queueName);
            
            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                yield return await GetPayload(receiver, serviceBusMessage, cancellationToken);
            }
        }

        public async Task<T> ReceiveMessageFromTopicSubscription<T>(string topic, string subscription, CancellationToken cancellationToken) 
            where T : class
        {
            await using var receiver = _client.CreateReceiver(topic, subscription);
            var serviceBusMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
            
            if (serviceBusMessage == null)
            {
                return null;
            }
            
            return await GetPayload<T>(receiver, serviceBusMessage, cancellationToken); 
        }

        public async Task<object> ReceiveMessageFromTopicSubscription(string topic, string subscription, CancellationToken cancellationToken)
        {
            await using var receiver = _client.CreateReceiver(topic, subscription);
            var serviceBusMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
            
            if (serviceBusMessage == null)
            {
                return null;
            }
            
            return await GetPayload(receiver, serviceBusMessage, cancellationToken); 
        }        
        
        public async IAsyncEnumerable<T> ReceiveMessagesFromTopicSubscription<T>(string topic, string subscription, CancellationToken cancellationToken) 
            where T : class
        {
            await using var receiver = _client.CreateReceiver(topic, subscription);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                yield return await GetPayload<T>(receiver, serviceBusMessage, cancellationToken);
            }
        }

        public async IAsyncEnumerable<object> ReceiveMessagesFromTopicSubscription(string topic, string subscription, CancellationToken cancellationToken)
        {
            await using var receiver = _client.CreateReceiver(topic, subscription);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                yield return await GetPayload(receiver, serviceBusMessage, cancellationToken);
            }
        }
        
        public async Task EnsureTopicSubscription(string topicName, string subscriptionName, CancellationToken cancellationToken, string sqlFilterRule = "1=1")
        {
            if (!await _administrationClient.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken))
            {
                var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName)
                {
                    Status = EntityStatus.Active,
                    MaxDeliveryCount = 10,
                    DefaultMessageTimeToLive = TimeSpan.FromDays(10000),
                    LockDuration = TimeSpan.FromSeconds(30),
                };

                var filterOptions = new CreateRuleOptions
                {
                    Name = "Default",
                    Filter = new SqlRuleFilter(string.IsNullOrWhiteSpace(sqlFilterRule) ? "1=1" : sqlFilterRule)
                };

                await _administrationClient.CreateSubscriptionAsync(subscriptionOptions, cancellationToken);
                await _administrationClient.CreateRuleAsync(topicName, subscriptionName, filterOptions, cancellationToken);
            }
        }        

        private async Task<T> GetPayload<T>(ServiceBusReceiver receiver, ServiceBusReceivedMessage serviceBusReceivedMessage, CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                var message = JsonConvert.DeserializeObject<ServiceBusMessage<T>>(serviceBusReceivedMessage.Body.ToString());
                await receiver.CompleteMessageAsync(serviceBusReceivedMessage, cancellationToken);
                return message?.Payload;
            }
            catch (Exception ex)
            {
                throw new ServiceBusSubscriberOperationException(ex.Message, ex);
            }
        }

        private async Task<object> GetPayload(ServiceBusReceiver receiver, ServiceBusReceivedMessage serviceBusReceivedMessage, CancellationToken cancellationToken)
        {
            if (!serviceBusReceivedMessage.ApplicationProperties.TryGetValue(nameof(Metadata.PayloadType), out var payloadTypeAsString) 
                || string.IsNullOrWhiteSpace((string)payloadTypeAsString)
                || !_serviceBusSubscriberConfiguration.DeserializationDictionary.TryGetValue((string)payloadTypeAsString, out var payloadType)
                || payloadType == null)
            {
                return null;
            }
            
            try
            {
                var message = JsonConvert.DeserializeObject(serviceBusReceivedMessage.Body.ToString(), payloadType);
                await receiver.CompleteMessageAsync(serviceBusReceivedMessage, cancellationToken);
                
                return message.GetType().GetProperty(nameof(ServiceBusMessage<object>.Payload)).GetValue(message, null);
            }
            catch (Exception ex)
            {
                throw new ServiceBusSubscriberOperationException(ex.Message, ex);
            }
        }
    }
}