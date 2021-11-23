using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusMessages;

namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriber : IServiceBusSubscriber
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _administrationClient;
        private readonly ServiceBusSubscriberConfiguration _serviceBusSubscriberConfiguration;
        private readonly ILogger<ServiceBusSubscriber> _logger;

        private ServiceBusSubscriberProcessor _serviceBusSubscriberProcessor;
        private Func<object, Task> _clientProcessMessageFunc;
        private Func<Exception, Task> _clientProcessErrorFunc;
        
        public ServiceBusSubscriber(
            ServiceBusClient client,
            ServiceBusAdministrationClient administrationClient,
            ServiceBusSubscriberConfiguration serviceBusSubscriberConfiguration,
            ILogger<ServiceBusSubscriber> logger)
        {
            _client = client;
            _administrationClient = administrationClient;
            _serviceBusSubscriberConfiguration = serviceBusSubscriberConfiguration;
            _logger = logger;
        }

        public async Task<T> ReceiveMessage<T>(
            string queueOrTopicName, 
            string subscriptionName = default,
            ServiceBusSubscriberReceiveOptions options = default, 
            CancellationToken cancellationToken = default)
            where T : class
        {
            await using var receiver = await GetServiceBusReceiver(queueOrTopicName, subscriptionName, options, cancellationToken);
            var serviceBusMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
            if (serviceBusMessage == null)
            {
                return null;
            }
            
            var returnFullMessage = options?.ReturnFullMessage ?? false;
            var payload = GetPayload<T>(serviceBusMessage, returnFullMessage, cancellationToken);
            await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
            return payload;
        }

        public async Task<object> ReceiveMessage(
            string queueOrTopicName,
            string subscriptionName = default,
            ServiceBusSubscriberReceiveOptions options = default,
            CancellationToken cancellationToken = default)
        {
            await using var receiver = await GetServiceBusReceiver(queueOrTopicName, subscriptionName, options, cancellationToken);
            var serviceBusMessage = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
            if (serviceBusMessage == null)
            {
                return null;
            }

            var returnFullMessage = options?.ReturnFullMessage ?? false;
            var payload = GetPayload(serviceBusMessage, returnFullMessage, cancellationToken);
            await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
            return payload;
        }

        public async IAsyncEnumerable<T> ReceiveMessages<T>(
            string queueOrTopicName, 
            string subscriptionName = default,
            ServiceBusSubscriberReceiveOptions options = default, 
            CancellationToken cancellationToken = default)
            where T : class
        {
            var returnFullMessage = options?.ReturnFullMessage ?? false;
            await using var receiver = await GetServiceBusReceiver(queueOrTopicName, subscriptionName, options, cancellationToken);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                var payload = GetPayload<T>(serviceBusMessage, returnFullMessage, cancellationToken);
                await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
                yield return payload;
            }
        }

        public async IAsyncEnumerable<object> ReceiveMessages(
            string queueOrTopicName,
            string subscriptionName = default, 
            ServiceBusSubscriberReceiveOptions options = default,
            CancellationToken cancellationToken = default)
        {
            var returnFullMessage = options?.ReturnFullMessage ?? false;
            await using var receiver = await GetServiceBusReceiver(queueOrTopicName, subscriptionName, options, cancellationToken);

            await foreach (var serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                var payload = GetPayload(serviceBusMessage, returnFullMessage, cancellationToken);
                await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
                yield return payload;
            }
        }

        public async Task StartReceiveMessages(
            string queueOrTopicName,
            string subscriptionName = default,
            Func<object, Task> processMessageFunc = default,
            Func<Exception, Task> processErrorFunc = default,
            ServiceBusSubscriberReceiveOptions options = default,
            CancellationToken cancellationToken = default)
        {
            if (_serviceBusSubscriberProcessor != null)
            {
                _logger.LogError("Service Bus processor already initialized. Stop it first");
                throw new ServiceBusSubscriberOperationException(
                    "Service Bus processor already initialized. Stop it first.");
            }

            try
            {
                _serviceBusSubscriberProcessor = new ServiceBusSubscriberProcessor(_client, queueOrTopicName, subscriptionName, options);
                
                _serviceBusSubscriberProcessor.ProcessMessageAsync += ProcessMessageInternal;
                _serviceBusSubscriberProcessor.ProcessErrorAsync += ProcessErrorInternal;

                _clientProcessMessageFunc = processMessageFunc;
                _clientProcessErrorFunc = processErrorFunc;

                await _serviceBusSubscriberProcessor.StartProcessingAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await StopReceiveMessagesFromTopicSubscriptionInternal(cancellationToken);
                
                _logger.LogError(ex, "Service Bus start receive messages from topic subscription failure");
                
                throw new ServiceBusSubscriberOperationException(
                    "Service Bus start receive messages from topic subscription failure", ex);
            }
        }

        public async Task StopReceiveMessagesFromTopicSubscription(CancellationToken cancellationToken)
        {
            if (_serviceBusSubscriberProcessor == null)
            {
                _logger.LogError("Service Bus processor not initialized");
                throw new ServiceBusSubscriberOperationException("Service Bus processor not initialized");
            }

            await StopReceiveMessagesFromTopicSubscriptionInternal(cancellationToken);
        }

        public async Task EnsureTopicSubscription(
            string topicName, 
            string subscriptionName, 
            CancellationToken cancellationToken, 
            string sqlFilterRule = "1=1")
        {
            try
            {
                if (!await _administrationClient.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken))
                {
                    const string defaultRuleName = "$Default";
                
                    var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName)
                    {
                        Status = EntityStatus.Active,
                        MaxDeliveryCount = 10,
                        LockDuration = TimeSpan.FromSeconds(60)
                    };

                    var filterOptions = new CreateRuleOptions
                    {
                        Name = defaultRuleName,
                        Filter = new SqlRuleFilter(string.IsNullOrWhiteSpace(sqlFilterRule) ? "1=1" : sqlFilterRule)
                    };

                    await _administrationClient.CreateSubscriptionAsync(subscriptionOptions, filterOptions, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus ensure topic subscription operation failure");
                
                throw new ServiceBusSubscriberOperationException(
                    "Service Bus ensure topic subscription operation failure", ex);
            }
        }     
        
        private object GetPayload(ServiceBusReceivedMessage serviceBusReceivedMessage, bool returnFullMessage, CancellationToken cancellationToken)
        {
            if (!serviceBusReceivedMessage.ApplicationProperties.TryGetValue(nameof(Metadata.EventName), out var eventNameAsObject)
                || !serviceBusReceivedMessage.ApplicationProperties.TryGetValue(nameof(Metadata.Version), out var versionAsObject)) 
            {
                return GetPayload<object>(serviceBusReceivedMessage, returnFullMessage, cancellationToken);
            }

            var eventName = (string)eventNameAsObject;
            var version = (int)versionAsObject;
            
            if (!_serviceBusSubscriberConfiguration.DeserializationDictionary.TryGetValue((version, eventName), out var payloadType)
                || payloadType == null)
            {
                return GetPayload<object>(serviceBusReceivedMessage, returnFullMessage, cancellationToken);
            }
            
            try
            {
                var message = JsonConvert.DeserializeObject(serviceBusReceivedMessage.Body.ToString(), payloadType);
                return returnFullMessage
                    ? message
                    : message.GetType().GetProperty(nameof(ServiceBusMessage<object>.Payload)).GetValue(message, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus received message get payload failure");
                throw new ServiceBusSubscriberOperationException("Service Bus received message get payload failure", ex);
            }
        }
        
        private T GetPayload<T>(ServiceBusReceivedMessage serviceBusReceivedMessage, bool returnFullMessage, CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                var message = JsonConvert.DeserializeObject(serviceBusReceivedMessage.Body.ToString(), typeof(ServiceBusMessage<T>));
                return returnFullMessage
                ? (T) message
                : ((ServiceBusMessage<T>)message)?.Payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus received message get payload failure");
                throw new ServiceBusSubscriberOperationException("Service Bus received message get payload failure", ex);
            }
        }       
        
        private async Task ProcessMessageInternal(object argsAsObject)
        {
            if (_clientProcessMessageFunc == null)
            {
                return;
            }

            var message = _serviceBusSubscriberProcessor.GetReceivedMessageFromArgs(argsAsObject);

            if (message == null)
            {
                await _clientProcessMessageFunc(null);
                return;
            }

            try
            {
                var payload = GetPayload(message, false, CancellationToken.None);
                await _clientProcessMessageFunc(payload);
                await _serviceBusSubscriberProcessor.CompleteMessageAsync(argsAsObject, message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus received message processing failure");
                await ProcessErrorInternal(new ServiceBusSubscriberOperationException("Service Bus received message processing failure", ex));
            }
        }

        private async Task ProcessErrorInternal(ProcessErrorEventArgs args)
        {
            await ProcessErrorInternal(args.Exception);
        }
        
        private async Task ProcessErrorInternal(Exception ex)
        {
            if (_clientProcessErrorFunc == null)
            {
                return;
            }

            await _clientProcessErrorFunc(ex);
        }
        
        private async Task StopReceiveMessagesFromTopicSubscriptionInternal(CancellationToken cancellationToken)
        {
            if (_serviceBusSubscriberProcessor != null)
            {
                try
                {
                    await _serviceBusSubscriberProcessor.CloseAsync(cancellationToken);
                    await _serviceBusSubscriberProcessor.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Service Bus receive messages stopping failure");
                }
                finally
                {
                    _serviceBusSubscriberProcessor = null;
                }                
            }
            
            _clientProcessMessageFunc = null;
            _clientProcessErrorFunc = null;
        }

        private async Task<ServiceBusReceiver> GetServiceBusReceiver(
            string queueOrTopicName, 
            string subscriptionName = default,
            ServiceBusSubscriberReceiveOptions options = default,
            CancellationToken cancellationToken = default)
        {
            var (sessionId, _) = (options?.SessionId, options?.ReturnFullMessage ?? false);

            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    return _client.CreateReceiver(queueOrTopicName);
                }

                return await _client.AcceptSessionAsync(queueOrTopicName, sessionId, cancellationToken: cancellationToken);
            }
            
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return _client.CreateReceiver(queueOrTopicName, subscriptionName);
            }
            
            return await _client.AcceptSessionAsync(queueOrTopicName, subscriptionName, sessionId, cancellationToken: cancellationToken);
        }
        
        private object GetServiceBusProcessor(
            string queueOrTopicName,
            string subscriptionName = default,
            ServiceBusSubscriberReceiveOptions options = default)
        {
            var (sessionId, _) = (options?.SessionId, options?.ReturnFullMessage ?? false);

            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    return _client.CreateProcessor(queueOrTopicName);
                }

                return _client.CreateSessionProcessor(
                    queueOrTopicName, 
                    new ServiceBusSessionProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        MaxConcurrentSessions = 5,
                        MaxConcurrentCallsPerSession = 2,
                        SessionIds = { sessionId }
                    });
            }
            
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return _client.CreateProcessor(queueOrTopicName, subscriptionName);
            }
            
            return _client.CreateSessionProcessor(
                queueOrTopicName, 
                subscriptionName, 
                new ServiceBusSessionProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentSessions = 5,
                    MaxConcurrentCallsPerSession = 2,                    
                    SessionIds = { sessionId }
                });
        }
    }
}