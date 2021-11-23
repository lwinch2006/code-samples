using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ILogger<TalentechAdminServiceBusClient> _logger;

        public TalentechAdminServiceBusClient(
            IServiceBusPublisher serviceBusPublisher,
            IServiceBusSubscriber serviceBusSubscriber,
            ILogger<TalentechAdminServiceBusClient> logger)
        {
            _serviceBusPublisher = serviceBusPublisher;
            _serviceBusSubscriber = serviceBusSubscriber;
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
            catch (Exception ex)
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
            catch (Exception ex)
            { }
        }

        public async Task StartReceiveMessagesFromTopicSubscription(string topic, string subscription, CancellationToken cancellationToken)
        {
            await _serviceBusSubscriber.StartReceiveMessages(
                topic,
                subscription,
                ProcessMessagesInternal,
                ProcessErrorsInternal,
                cancellationToken: cancellationToken
            );
        }

        public async Task StopReceiveMessagesFromTopicSubscription(CancellationToken cancellationToken)
        {
            await _serviceBusSubscriber.StopReceiveMessagesFromTopicSubscription(cancellationToken);
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
                    Timestamp = DateTime.UtcNow
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
                var responseAsObject = await _serviceBusSubscriber.ReceiveMessage(
                    responseQueue, 
                    options: new ServiceBusSubscriberReceiveOptions { SessionId = sessionId, ReturnFullMessage = true}, 
                    cancellationToken: CancellationToken.None);
                
                var response = (ServiceBusMessage<Response>)responseAsObject;
                _logger.LogInformation("Received response {EventFullName} for request with status {StatusCode} and message {Message}", response.Metadata.EventName, response.Payload.StatusCode, response.Payload.Message);
            }
            catch (Exception ex)
            { }
        }

        public async Task ReceiveRequestsAndSendResponses(string requestQueue, string responseQueue)
        {
            var source = new CancellationTokenSource();

            try
            {
                await foreach (var receivedRequest in _serviceBusSubscriber.ReceiveMessages(
                    requestQueue, 
                    options: new ServiceBusSubscriberReceiveOptions { ReturnFullMessage = true },
                    cancellationToken: source.Token))
                {
                    switch (receivedRequest)
                    {
                        case ServiceBusMessage<TenantUpdated> tenantUpdatedEvent:
                            _logger.LogInformation("Received queue {Event} request - tenant with Id {TenantId} changed name to {NewName}", nameof(TenantUpdated),  tenantUpdatedEvent.Payload.Id, tenantUpdatedEvent.Payload.NewName);
                            await SendResponse(responseQueue, tenantUpdatedEvent);
                            break;
                    
                        case ServiceBusMessage<UserUpdated> userUpdatedEvent:
                            _logger.LogInformation("Received queue {Event} request - tenant with Id {TenantId} changed name to {NewName}", nameof(UserUpdated),  userUpdatedEvent.Payload.Id, userUpdatedEvent.Payload.NewName);
                            await SendResponse(responseQueue, userUpdatedEvent);
                            break;                    
                    
                        default:
                            _logger.LogInformation("Received queue {Event} request - raw data {RawData}", "unknown", receivedRequest?.ToString()?.Replace(Environment.NewLine, string.Empty));
                            break;
                    }
                
                    //source.Cancel();
                }
            }
            catch (Exception ex)
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
                    Timestamp = DateTime.UtcNow
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
            catch (Exception ex)
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
                    Timestamp = DateTime.UtcNow
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
            catch (Exception ex)
            { }
        }

        private Task ProcessMessagesInternal(object payload)
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

        private Task ProcessErrorsInternal(Exception exception)
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
                    EventType = AppConstants.Events.TenantEvents.Type,
                    EventName = AppConstants.Events.TenantEvents.TenantUpdatedResponse.FullName,
                    Version = 1,
                    Timestamp = DateTime.UtcNow
                },
                Payload = new Response
                {
                    StatusCode = 200,
                    Message = "OK"
                },
                SessionId = message.ReplyToSessionId
            };
            
            await _serviceBusPublisher.SendMessage(responseQueue, response, CancellationToken.None);
        }
    }
}