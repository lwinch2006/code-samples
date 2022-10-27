using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriberProcessor : IAsyncDisposable
    {
        private readonly object _serviceBusProcessorAsObject;

        public string QueueOrTopicName { get; init; }
        public string? SubscriptionName { get; init; }
        public ServiceBusSubscriberReceiveOptions ServiceBusSubscriberReceiveOptions { get; }
        public Func<string, string?, object?, Task>? ClientProcessMessageFunc { get; init; }
        public Func<string, string?, Exception, Task>? ClientProcessErrorFunc { get; init; }

        public ServiceBusSubscriberProcessor(ServiceBusClient client,
            string queueOrTopicName, 
            string? subscriptionName = default,
            ServiceBusSubscriberReceiveOptions? options = default)
        {
            QueueOrTopicName = queueOrTopicName;
            SubscriptionName = subscriptionName;
            ServiceBusSubscriberReceiveOptions = options ?? new ServiceBusSubscriberReceiveOptions();
            
            var (sessionId, _, connectToDeadLetterQueue) = (options?.SessionId, ServiceBusSubscriberReceiveMessageTypes.Payload, options?.ConnectToDeadLetterQueue ?? false);

            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    _serviceBusProcessorAsObject = client.CreateProcessor(
                        queueOrTopicName,
                        new ServiceBusProcessorOptions
                        {
                            AutoCompleteMessages = false,
                            SubQueue = connectToDeadLetterQueue ? SubQueue.DeadLetter : SubQueue.None
                        });
                    
                    return;
                }

                _serviceBusProcessorAsObject = client.CreateSessionProcessor(
                    queueOrTopicName, 
                    new ServiceBusSessionProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        MaxConcurrentSessions = 5,
                        MaxConcurrentCallsPerSession = 2,
                        SessionIds = { sessionId }
                    });
                
                return;
            }
            
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                _serviceBusProcessorAsObject = client.CreateProcessor(
                    queueOrTopicName, 
                    subscriptionName,
                    new ServiceBusProcessorOptions
                    {
                        AutoCompleteMessages = false,
                        SubQueue = connectToDeadLetterQueue ? SubQueue.DeadLetter : SubQueue.None
                    });
                
                return;
            }
            
            _serviceBusProcessorAsObject = client.CreateSessionProcessor(
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

        public event Func<object, Task> ProcessMessageAsync
        {
            add
            {
                switch (_serviceBusProcessorAsObject)
                {
                    case ServiceBusProcessor serviceBusProcessor:
                        serviceBusProcessor.ProcessMessageAsync += value;
                        break;
                    
                    case ServiceBusSessionProcessor serviceBusSessionProcessor:
                        serviceBusSessionProcessor.ProcessMessageAsync += value;
                        break;
                }
            }

            remove
            {
                switch (_serviceBusProcessorAsObject)
                {
                    case ServiceBusProcessor serviceBusProcessor:
                        serviceBusProcessor.ProcessMessageAsync -= value;
                        break;
                    
                    case ServiceBusSessionProcessor serviceBusSessionProcessor:
                        serviceBusSessionProcessor.ProcessMessageAsync -= value;
                        break;
                }
            }
        }
        
        public event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync
        {
            add
            {
                switch (_serviceBusProcessorAsObject)
                {
                    case ServiceBusProcessor serviceBusProcessor:
                        serviceBusProcessor.ProcessErrorAsync += value;
                        break;
                    
                    case ServiceBusSessionProcessor serviceBusSessionProcessor:
                        serviceBusSessionProcessor.ProcessErrorAsync += value;
                        break;
                }
            }

            remove
            {
                switch (_serviceBusProcessorAsObject)
                {
                    case ServiceBusProcessor serviceBusProcessor:
                        serviceBusProcessor.ProcessErrorAsync -= value;
                        break;
                    
                    case ServiceBusSessionProcessor serviceBusSessionProcessor:
                        serviceBusSessionProcessor.ProcessErrorAsync -= value;
                        break;
                }
            }
        }        

        public Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            return _serviceBusProcessorAsObject switch
            {
                ServiceBusProcessor serviceBusProcessor => serviceBusProcessor.StartProcessingAsync(cancellationToken),
                ServiceBusSessionProcessor serviceBusSessionProcessor => serviceBusSessionProcessor.StartProcessingAsync(cancellationToken),
                _ => Task.CompletedTask
            };
        }
        
        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            return _serviceBusProcessorAsObject switch
            {
                ServiceBusProcessor serviceBusProcessor => serviceBusProcessor.CloseAsync(cancellationToken),
                ServiceBusSessionProcessor serviceBusSessionProcessor => serviceBusSessionProcessor.CloseAsync(cancellationToken),
                _ => Task.CompletedTask
            };
        }

        public ServiceBusReceivedMessage GetReceivedMessageFromArgs(object argsAsObject)
        {
            return argsAsObject switch
            {
                ProcessMessageEventArgs args => args.Message,
                ProcessSessionMessageEventArgs args => args.Message,
            };
        }

        public Task CompleteMessageAsync(object argsAsObject, CancellationToken cancellationToken = default)
        {
            return argsAsObject switch
            {
                ProcessMessageEventArgs args => args.CompleteMessageAsync(args.Message, cancellationToken),
                ProcessSessionMessageEventArgs args => args.CompleteMessageAsync(args.Message, cancellationToken),
            };
        }

        public Task AbandonMessageAsync(
            object argsAsObject, 
            IDictionary<string, object>? propertiesToModify = default,
            CancellationToken cancellationToken = default)
        {
            return argsAsObject switch
            {
                ProcessMessageEventArgs args => args.AbandonMessageAsync(args.Message, propertiesToModify, cancellationToken),
                ProcessSessionMessageEventArgs args => args.AbandonMessageAsync(args.Message, propertiesToModify, cancellationToken),
                _ => Task.CompletedTask
            };
        }        
        
        public Task DeadLetterMessageAsync(
            object argsAsObject, 
            string deadLetterReason,
            string? deadLetterErrorDescription = default, 
            CancellationToken cancellationToken = default)
        {
            return argsAsObject switch
            {
                ProcessMessageEventArgs args => args.DeadLetterMessageAsync(args.Message, deadLetterReason, deadLetterErrorDescription,
                    cancellationToken),
                ProcessSessionMessageEventArgs args => args.DeadLetterMessageAsync(args.Message, deadLetterReason, deadLetterErrorDescription,
                    cancellationToken),
                _ => Task.CompletedTask
            };
        }
        
        public ValueTask DisposeAsync()
        {
            return _serviceBusProcessorAsObject switch
            {
                ServiceBusProcessor serviceBusProcessor => serviceBusProcessor.DisposeAsync(),
                ServiceBusSessionProcessor serviceBusSessionProcessor => serviceBusSessionProcessor.DisposeAsync(),
                _ => ValueTask.CompletedTask
            };
        }        
    }
}