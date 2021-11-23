using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriberProcessor : IAsyncDisposable
    {
        private readonly object _serviceBusProcessorAsObject;

        public ServiceBusSubscriberProcessor(ServiceBusClient client,
            string queueOrTopicName, 
            string subscriptionName = default,
            ServiceBusSubscriberReceiveOptions options = default)
        {
            var (sessionId, _) = (options?.SessionId, options?.ReturnFullMessage ?? false);

            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    _serviceBusProcessorAsObject = client.CreateProcessor(queueOrTopicName);
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
                _serviceBusProcessorAsObject = client.CreateProcessor(queueOrTopicName, subscriptionName);
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

        public Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            switch (_serviceBusProcessorAsObject)
            {
                case ServiceBusProcessor serviceBusProcessor:
                    return serviceBusProcessor.StartProcessingAsync(cancellationToken);
                
                case ServiceBusSessionProcessor serviceBusSessionProcessor:
                    return serviceBusSessionProcessor.StartProcessingAsync(cancellationToken);
                
                default:
                    return Task.CompletedTask;
            }
        }
        
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return _serviceBusProcessorAsObject switch
            {
                ServiceBusProcessor serviceBusProcessor => serviceBusProcessor.CloseAsync(cancellationToken),
                ServiceBusSessionProcessor serviceBusSessionProcessor => serviceBusSessionProcessor.CloseAsync(cancellationToken),
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

        public ServiceBusReceivedMessage GetReceivedMessageFromArgs(object args)
        {
            var message = _serviceBusProcessorAsObject switch
            {
                ServiceBusProcessor => ((ProcessMessageEventArgs)args).Message,
                ServiceBusSessionProcessor => ((ProcessSessionMessageEventArgs)args).Message,
                _ => null
            };

            return message;
        }

        public Task CompleteMessageAsync(object args, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            switch (args)
            {
                case ServiceBusProcessor:
                    return ((ProcessMessageEventArgs)args).CompleteMessageAsync(message, cancellationToken);
                
                case ServiceBusSessionProcessor:
                    return ((ProcessSessionMessageEventArgs)args).CompleteMessageAsync(message, cancellationToken);
                
                default:
                    return Task.CompletedTask;
            }
        }
        
    }
}