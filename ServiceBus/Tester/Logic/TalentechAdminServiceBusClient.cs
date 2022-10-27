using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using ServiceBusMessages;
using ServiceBusPublisher;
using ServiceBusSubscriber;
using ServiceBusTester.Models;
using ServiceBusTester.Models.Events.V1.Tenant;
using ServiceBusTester.Models.Events.V1.User;
using ServiceBusTester.Models.Responses;

namespace ServiceBusTester.Logic
{
    public class TalentechAdminServiceBusClient : ITalentechAdminServiceBusClient
    {
        private readonly IServiceBusPublisher _serviceBusPublisher;
        private readonly IServiceBusSubscriber _serviceBusSubscriber;
        private readonly IServiceBusSubscriberForDeadLetter _serviceBusSubscriberForDeadLetter;
        private readonly ILogger<TalentechAdminServiceBusClient> _logger;

        public TalentechAdminServiceBusClient(
            IServiceBusPublisher serviceBusPublisher,
            IServiceBusSubscriber serviceBusSubscriber,
            IServiceBusSubscriberForDeadLetter serviceBusSubscriberForDeadLetter,
            ILogger<TalentechAdminServiceBusClient> logger)
        {
            _serviceBusPublisher = serviceBusPublisher;
            _serviceBusSubscriber = serviceBusSubscriber;
            _serviceBusSubscriberForDeadLetter = serviceBusSubscriberForDeadLetter;
            _logger = logger;
        }
        
        public async Task SendTenantChangedEvent(string queueOrTopic)
        {
            await SendSampleTenantEvent(queueOrTopic);
        }

        public async Task SendUserChangedEvent(string queueOrTopic)
        {
            await SendSampleUserEvent(queueOrTopic);
        }
        
        public async Task ReceiveEventsFromQueue(string queue)
        {
            var source = new CancellationTokenSource();

            try
            {
                await foreach (var receivedEvent in _serviceBusSubscriber.ReceiveMessages(queue, cancellationToken: source.Token))
                {
                    switch (receivedEvent)
                    {
                        case TenantUpdated tenantUpdatedEvent:
                            _logger.LogInformation("Received queue {Event} event - tenant with Id {TenantId} changed name to {NewName}", nameof(TenantUpdated),  tenantUpdatedEvent.Id, tenantUpdatedEvent.NewName);
                            break;
                    
                        case UserUpdated userUpdatedEvent:
                            _logger.LogInformation("Received queue {Event} event - tenant with Id {TenantId} changed name to {NewName}", nameof(UserUpdated),  userUpdatedEvent.Id, userUpdatedEvent.NewName);
                            break;                    
                    
                        default:
                            _logger.LogInformation("Received queue {Event} event - raw data {RawData}", "unknown", receivedEvent?.ToString()?.Replace(Environment.NewLine, string.Empty));
                            break;
                    }
                
                    //source.Cancel();
                }
            }
            catch (Exception)
            { }
        }
        
        public async Task ReceiveEventsFromTopicSubscription(string topic, string subscription)
        {
            var source = new CancellationTokenSource();
            
            try
            {
                await foreach (var receivedEvent in _serviceBusSubscriber.ReceiveMessages(topic, subscription, cancellationToken: source.Token))
                {
                    switch (receivedEvent)
                    {
                        case TenantUpdated tenantUpdatedEvent:
                            _logger.LogInformation("Received topic/subscription {Event} event - tenant with Id {TenantId} changed name to {NewName}", nameof(TenantUpdated),  tenantUpdatedEvent.Id, tenantUpdatedEvent.NewName);
                            break;
                    
                        case UserUpdated userUpdatedEvent:
                            _logger.LogInformation("Received topic/subscription {Event} event - tenant with Id {TenantId} changed name to {NewName}", nameof(UserUpdated),  userUpdatedEvent.Id, userUpdatedEvent.NewName);
                            break;                    
                    
                        default:
                            _logger.LogInformation("Received topic/subscription {Event} event - raw data {RawData}", "unknown", receivedEvent?.ToString()?.Replace(Environment.NewLine, string.Empty));
                            break;
                    }

                    //source.Cancel();
                }
            }
            catch (Exception)
            { }
        }

        public async Task StartReceiveMessages(string topic, string subscription, CancellationToken cancellationToken)
        {
            await _serviceBusSubscriber.StartReceiveMessages(
                topic,
                subscription,
                ProcessMessagesInternal,
                ProcessErrorsInternal,
                cancellationToken: cancellationToken
            );
        }

        public async Task StopReceiveMessages(CancellationToken cancellationToken)
        {
            await _serviceBusSubscriber.StopReceiveMessages(cancellationToken);
        }

        public async Task StartReceiveMessagesFromDeadLetterQueue(string topic, string subscription, CancellationToken cancellationToken)
        {
            await _serviceBusSubscriberForDeadLetter.StartReceiveMessages(
                topic,
                subscription,
                ProcessFullMessagesInDeadLetterQueueInternal,
                ProcessErrorsInDeadLetterQueueInternal,
                options: new ServiceBusSubscriberReceiveOptions
                {
                    ReceiveMessageType = ServiceBusSubscriberReceiveMessageTypes.FullMessage,
                    ConnectToDeadLetterQueue = true
                },
                cancellationToken: cancellationToken
            );            
        }
        
        public async Task StopReceiveMessagesFromDeadLetterQueue(CancellationToken cancellationToken)
        {
            await _serviceBusSubscriberForDeadLetter.StopReceiveMessages(cancellationToken);
        }
        
        public async Task SendRequestAndWaitForResponse(string requestQueue, string responseQueue)
        {
            var sessionId = Guid.NewGuid().ToString();
            
            var message = new ServiceBusMessage<TenantUpdated>
            {
                Metadata = new Metadata
                {
                    EventType = AppConstants.Events.TenantEvents.Type,
                    EventName = AppConstants.Events.TenantEvents.TenantUpdated.FullName,
                    Version = 1,
                    CreatedOnUtc = DateTime.UtcNow
                },
                Payload = new TenantUpdated
                {
                    Id = Guid.NewGuid(),
                    NewName = "SampleTenantName"
                },
                ReplyToSessionId = sessionId
            };
            
            try
            {
                await _serviceBusPublisher.SendMessage(requestQueue, message, CancellationToken.None);
                var responseAsObject = (await _serviceBusSubscriber.ReceiveMessage(
                    responseQueue, 
                    options: new ServiceBusSubscriberReceiveOptions { SessionId = sessionId, ReceiveMessageType = ServiceBusSubscriberReceiveMessageTypes.Message}, 
                    cancellationToken: CancellationToken.None))!;
                
                var response = (ServiceBusMessage<Response>)responseAsObject;
                _logger.LogInformation("Received response for request on event {EventFullName} with status {StatusCode} and message {Message}", response.Payload.EventName, response.Payload.StatusCode, response.Payload.Message);
            }
            catch (Exception)
            { }
        }

        public async Task ReceiveRequestsAndSendResponses(string requestQueue, string responseQueue)
        {
            var source = new CancellationTokenSource();

            try
            {
                await foreach (var receivedRequest in _serviceBusSubscriber.ReceiveMessages(
                    requestQueue, 
                    options: new ServiceBusSubscriberReceiveOptions { ReceiveMessageType = ServiceBusSubscriberReceiveMessageTypes.Message },
                    cancellationToken: source.Token))
                {
                    switch (receivedRequest)
                    {
                        case ServiceBusMessage<TenantUpdated> tenantUpdatedEvent:
                            _logger.LogInformation("Received request on event {Event} - tenant with Id {TenantId} changed name to {NewName}", tenantUpdatedEvent.Metadata.EventName,  tenantUpdatedEvent.Payload.Id, tenantUpdatedEvent.Payload.NewName);
                            await SendResponse(responseQueue, tenantUpdatedEvent);
                            break;
                    
                        case ServiceBusMessage<UserUpdated> userUpdatedEvent:
                            _logger.LogInformation("Received request on event {Event} - tenant with Id {TenantId} changed name to {NewName}", userUpdatedEvent.Metadata.EventName,  userUpdatedEvent.Payload.Id, userUpdatedEvent.Payload.NewName);
                            await SendResponse(responseQueue, userUpdatedEvent);
                            break;                    
                    
                        default:
                            _logger.LogInformation("Received request on event {Event} - raw data {RawData}", "unknown", receivedRequest?.ToString()?.Replace(Environment.NewLine, string.Empty));
                            break;
                    }
                
                    //source.Cancel();
                }
            }
            catch (Exception)
            { }
        }
        
        private async Task SendSampleTenantEvent(string queueOrTopic)
        {
            var message = new ServiceBusMessage<TenantUpdated>
            {
                Metadata = new Metadata
                {
                    EventType = AppConstants.Events.TenantEvents.Type,
                    EventName = AppConstants.Events.TenantEvents.TenantUpdated.FullName,
                    Version = 1,
                    CreatedOnUtc = DateTime.UtcNow
                },
                Payload = new TenantUpdated
                {
                    Id = Guid.NewGuid(),
                    NewName = "SampleTenantName"
                }
            };

            try
            {
                await _serviceBusPublisher
                    .SendMessage(queueOrTopic, message, CancellationToken.None);
            }
            catch (Exception)
            { }
        }

        private async Task SendSampleUserEvent(string queueOrTopic)
        {
            var message = new ServiceBusMessage<UserUpdated>
            {
                Metadata = new Metadata
                {
                    EventType = AppConstants.Events.UserEvents.Type,
                    EventName = AppConstants.Events.UserEvents.UserUpdated.FullName,
                    Version = 1,
                    CreatedOnUtc = DateTime.UtcNow
                },
                Payload = new UserUpdated
                {
                    Id = Guid.NewGuid(),
                    NewName = "SampleUserName"
                }
            };

            try
            {
                await _serviceBusPublisher.SendMessage(queueOrTopic, message, CancellationToken.None); 
            }
            catch (Exception)
            { }
        }

        private Task ProcessMessagesInternal(string queueOrTopicName, string? subscriptionName, object? payload)
        {
            switch (payload)
            {
                case TenantUpdated tenantUpdatedEvent:
                    _logger.LogInformation("Received topic/subscription {Event} event - tenant with Id {TenantId} changed name to {NewName}", nameof(TenantUpdated),  tenantUpdatedEvent.Id, tenantUpdatedEvent.NewName);
                    break;
                    
                case UserUpdated userUpdatedEvent:
                    _logger.LogInformation("Received topic/subscription {Event} event - tenant with Id {TenantId} changed name to {NewName}", nameof(UserUpdated),  userUpdatedEvent.Id, userUpdatedEvent.NewName);
                    break;                    
                    
                default:
                    _logger.LogInformation("Received topic/subscription {Event} event - raw data {RawData}", "unknown", payload?.ToString()?.Replace(Environment.NewLine, string.Empty));
                    break;
            }

            return Task.CompletedTask;
        }
        
        private Task ProcessErrorsInternal(string queueOrTopicName, string? subscriptionName, Exception exception)
        {
            _logger.LogError(exception, "Service Bus receive message error");
            return Task.CompletedTask;
        }        

        private async Task ProcessMessagesInDeadLetterQueueInternal(string queueOrTopicName, string? subscriptionName, object? message)
        {
            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                _logger.LogInformation("Received {Event} event from dead letter subqueue of {Queue} queue - raw data {RawData}", "unknown", queueOrTopicName, message?.ToString()?.Replace(Environment.NewLine, string.Empty));
            }
            else
            {
                _logger.LogInformation("Received {Event} event from dead letter subqueue of {Topic}/{Subscription} topic/subscription - raw data {RawData}", "unknown", queueOrTopicName, subscriptionName, message?.ToString()?.Replace(Environment.NewLine, string.Empty));
            }
            
            await _serviceBusPublisher.SendMessage(queueOrTopicName, ServiceBusMessage<object>.FromObject(message)!, CancellationToken.None);
        }
        
        private async Task ProcessFullMessagesInDeadLetterQueueInternal(string queueOrTopicName, string? subscriptionName, object? messageAsObject)
        {
            var message = (ServiceBusReceivedMessage) messageAsObject!;
            
            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                _logger.LogInformation("Received {Event} event from dead letter subqueue of {Queue} queue - raw data {RawData}", "unknown", queueOrTopicName, message.ToString()?.Replace(Environment.NewLine, string.Empty));
            }
            else
            {
                _logger.LogInformation("Received {Event} event from dead letter subqueue of {Topic}/{Subscription} topic/subscription - raw data {RawData}", "unknown", queueOrTopicName, subscriptionName, message.ToString()?.Replace(Environment.NewLine, string.Empty));
            }
            
            await _serviceBusPublisher.SendMessage(queueOrTopicName, message, CancellationToken.None);
        }        

        private Task ProcessErrorsInDeadLetterQueueInternal(string queueOrTopicName, string? subscriptionName, Exception exception)
        {
            _logger.LogError(exception, "Service Bus receive message error");
            return Task.CompletedTask;
        }

        private async Task SendResponse<T>(string responseQueue, ServiceBusMessage<T> message)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(message.ReplyToSessionId))
            {
                return;
            }
            
            var response = new ServiceBusMessage<Response>
            {
                Metadata = new Metadata
                {
                    EventType = AppConstants.Events.Responses.Type,
                    EventName = AppConstants.Events.Responses.GeneralResponse.FullName,
                    Version = 1,
                    CreatedOnUtc = DateTime.UtcNow
                },
                Payload = new Response
                {
                    EventName = message.Metadata.EventName,
                    StatusCode = 200,
                    Message = "OK"
                },
                SessionId = message.ReplyToSessionId
            };
            
            await _serviceBusPublisher.SendMessage(responseQueue, response, CancellationToken.None);
        }
    }
}