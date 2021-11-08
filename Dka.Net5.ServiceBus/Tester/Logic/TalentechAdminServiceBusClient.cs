﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceBusMessages;
using ServiceBusPublisher;
using ServiceBusSubscriber;
using ServiceBusTester.Models.Events.V1.Tenant;
using ServiceBusTester.Models.Events.V1.User;

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
                await foreach (var receivedEvent in _serviceBusSubscriber.ReceiveMessagesFromQueue(queue, source.Token))
                {
                    switch (receivedEvent)
                    {
                        case TenantUpdated tenantUpdatedEvent:
                            _logger.LogInformation("Received queue {Event} event - tenant with Id {TenantId} changed name to {NewName}", $"{nameof(TenantUpdated)}",  tenantUpdatedEvent.Id, tenantUpdatedEvent.NewName);
                            break;
                    
                        case UserUpdated userUpdatedEvent:
                            _logger.LogInformation("Received queue {Event} event - tenant with Id {TenantId} changed name to {NewName}", $"{nameof(UserUpdated)}",  userUpdatedEvent.Id, userUpdatedEvent.NewName);
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
                await foreach (var receivedEvent in _serviceBusSubscriber.ReceiveMessagesFromTopicSubscription(topic, subscription, source.Token))
                {
                    switch (receivedEvent)
                    {
                        case TenantUpdated tenantUpdatedEvent:
                            _logger.LogInformation("Received topic/subscription {Event} event - tenant with Id {TenantId} changed name to {NewName}", $"{nameof(TenantUpdated)}",  tenantUpdatedEvent.Id, tenantUpdatedEvent.NewName);
                            break;
                    
                        case UserUpdated userUpdatedEvent:
                            _logger.LogInformation("Received topic/subscription {Event} event - tenant with Id {TenantId} changed name to {NewName}", $"{nameof(UserUpdated)}",  userUpdatedEvent.Id, userUpdatedEvent.NewName);
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
        
        private async Task SendSampleTenantEvent(string queueOrTopic)
        {
            var message = new ServiceBusMessage<TenantUpdated>
            {
                Metadata = new Metadata
                {
                    EventType = "TenantEvents",
                    EventName = nameof(TenantUpdated),
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
                    EventType = "UserEvents",
                    EventName = nameof(UserUpdated),
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
    }
}