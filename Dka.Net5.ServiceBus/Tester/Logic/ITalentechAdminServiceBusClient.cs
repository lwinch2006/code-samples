using System.Threading.Tasks;

namespace ServiceBusTester.Logic
{
    public interface ITalentechAdminServiceBusClient
    {
        Task SendTenantChangedEvent(string queueOrTopic);
        
        Task SendUserChangedEvent(string queueOrTopic);

        Task ReceiveEventsFromQueue(string queue);

        Task ReceiveEventsFromTopicSubscription(string topic, string subscription);
    }
}