using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusTester.Logic
{
    public interface ITalentechAdminServiceBusClient
    {
        Task SendTenantChangedEvent(string queueOrTopic);
        
        Task SendUserChangedEvent(string queueOrTopic);

        Task ReceiveEventsFromQueue(string queue);

        Task ReceiveEventsFromTopicSubscription(string topic, string subscription);

        Task StartReceiveMessages(string topic, string subscription, CancellationToken cancellationToken);

        Task StopReceiveMessages(CancellationToken cancellationToken);

        Task SendRequestAndWaitForResponse(string requestQueue, string responseQueue);

        Task ReceiveRequestsAndSendResponses(string requestQueue, string responseQueue);
        
        Task StartReceiveMessagesFromDeadLetterQueue(string topic, string subscription,
            CancellationToken cancellationToken);
        
        Task StopReceiveMessagesFromDeadLetterQueue(CancellationToken cancellationToken);
    }
}