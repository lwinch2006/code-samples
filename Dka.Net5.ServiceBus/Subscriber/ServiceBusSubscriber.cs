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

        private ServiceBusProcessor _serviceBusProcessor;
        private Func<object, Task> _clientProcessMessageFunc;
        private Func<Exception, Task> _clientProcessErrorFunc;
        
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

            var payload = GetPayload<T>(serviceBusMessage, cancellationToken);
            await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
            return payload;
        }
        
        public async IAsyncEnumerable<T> ReceiveMessagesFromQueue<T>(string queueName, CancellationToken cancellationToken) 
            where T : class
        {
            await using var receiver = _client.CreateReceiver(queueName);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                 var payload = GetPayload<T>(serviceBusMessage, cancellationToken);
                 await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
                 yield return payload;
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
            
            var payload = GetPayload(serviceBusMessage, cancellationToken);
            await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
            return payload;
        }

        public async IAsyncEnumerable<object> ReceiveMessagesFromQueue(string queueName, CancellationToken cancellationToken)
        {
            await using var receiver = _client.CreateReceiver(queueName);
            
            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                var payload = GetPayload(serviceBusMessage, cancellationToken);
                await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
                yield return payload;
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
            
            var payload = GetPayload<T>(serviceBusMessage, cancellationToken);
            await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
            return payload;
        }

        public async Task<object> ReceiveMessageFromTopicSubscription(string topic, string subscription, CancellationToken cancellationToken)
        {
            await using var receiver = _client.CreateReceiver(topic, subscription);
            var serviceBusMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
            
            if (serviceBusMessage == null)
            {
                return null;
            }
            
            var payload = GetPayload(serviceBusMessage, cancellationToken);
            await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
            return payload;
        }        
        
        public async IAsyncEnumerable<T> ReceiveMessagesFromTopicSubscription<T>(string topic, string subscription, CancellationToken cancellationToken) 
            where T : class
        {
            await using var receiver = _client.CreateReceiver(topic, subscription);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                 var payload = GetPayload<T>(serviceBusMessage, cancellationToken);
                 await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
                 yield return payload;
            }
        }

        public async IAsyncEnumerable<object> ReceiveMessagesFromTopicSubscription(string topic, string subscription, CancellationToken cancellationToken)
        {
            await using var receiver = _client.CreateReceiver(topic, subscription);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                var payload = GetPayload(serviceBusMessage, cancellationToken);
                await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
                yield return payload;
            }
        }

        public async Task StartReceiveMessagesFromTopicSubscription(
            string topic, 
            string subscription, 
            Func<object, Task> processMessageFunc,
            Func<Exception, Task> processErrorFunc,
            CancellationToken cancellationToken)
        {
            if (_serviceBusProcessor != null)
            {
                throw new ServiceBusSubscriberOperationException(
                    "Service Bus processor already initialized. Stop it first.");
            }

            _serviceBusProcessor = _client.CreateProcessor(topic, subscription);
            _serviceBusProcessor.ProcessMessageAsync += ProcessMessageInternal;
            _serviceBusProcessor.ProcessErrorAsync += ProcessErrorInternal;

            _clientProcessMessageFunc = processMessageFunc;
            _clientProcessErrorFunc = processErrorFunc;
            
            await _serviceBusProcessor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopReceiveMessagesFromTopicSubscription(CancellationToken cancellationToken)
        {
            if (_serviceBusProcessor == null)
            {
                throw new ServiceBusSubscriberOperationException("Service Bus processor not initialized");
            }

            await _serviceBusProcessor.CloseAsync(cancellationToken);
            await _serviceBusProcessor.DisposeAsync();

            _clientProcessMessageFunc = null;
            _clientProcessErrorFunc = null;
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
        
        private object GetPayload(ServiceBusReceivedMessage serviceBusReceivedMessage, CancellationToken cancellationToken)
        {
            if (!serviceBusReceivedMessage.ApplicationProperties.TryGetValue(nameof(Metadata.EventType), out var eventTypeAsObject)
                || !serviceBusReceivedMessage.ApplicationProperties.TryGetValue(nameof(Metadata.EventName), out var eventNameAsObject)
                || !serviceBusReceivedMessage.ApplicationProperties.TryGetValue(nameof(Metadata.Version), out var versionAsObject)) 
            {
                return GetPayload<object>(serviceBusReceivedMessage, cancellationToken);
            }

            var eventFullName = $"{eventTypeAsObject}.{eventNameAsObject}";
            var version = (int)versionAsObject;
            
            if (!_serviceBusSubscriberConfiguration.DeserializationDictionary.TryGetValue((version, eventFullName), out var payloadType)
                || payloadType == null)
            {
                return GetPayload<object>(serviceBusReceivedMessage, cancellationToken);
            }
            
            try
            {
                var message = JsonConvert.DeserializeObject(serviceBusReceivedMessage.Body.ToString(), payloadType);
                return message.GetType().GetProperty(nameof(ServiceBusMessage<object>.Payload)).GetValue(message, null);
            }
            catch (Exception ex)
            {
                throw new ServiceBusSubscriberOperationException(ex.Message, ex);
            }
        }        
        
        private T GetPayload<T>(ServiceBusReceivedMessage serviceBusReceivedMessage, CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                var message = JsonConvert.DeserializeObject<ServiceBusMessage<T>>(serviceBusReceivedMessage.Body.ToString());
                return message?.Payload;
            }
            catch (Exception ex)
            {
                throw new ServiceBusSubscriberOperationException(ex.Message, ex);
            }
        }        
        
        private async Task ProcessMessageInternal(ProcessMessageEventArgs args)
        {
            if (_clientProcessMessageFunc == null)
            {
                return;
            }
            
            if (args.Message == null)
            {
                await _clientProcessMessageFunc(null);
            }
            
            var payload = GetPayload(args.Message, CancellationToken.None);
            await _clientProcessMessageFunc(payload);
            await args.CompleteMessageAsync(args.Message, CancellationToken.None);
        }

        private async Task ProcessErrorInternal(ProcessErrorEventArgs args)
        {
            if (_clientProcessErrorFunc == null)
            {
                return;
            }

            await _clientProcessErrorFunc(args.Exception);
        }
    }
}