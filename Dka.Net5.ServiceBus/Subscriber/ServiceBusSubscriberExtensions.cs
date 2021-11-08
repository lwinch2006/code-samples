using System.Threading;

namespace ServiceBusSubscriber
{
    public static class ServiceBusSubscriberExtensions
    {
        public static IServiceBusSubscriber EnsureTopicSubscriptionEx(this IServiceBusSubscriber serviceBusSubscriber, string topicName, string subscriptionName, CancellationToken cancellationToken)
        {
            serviceBusSubscriber.EnsureTopicSubscription(topicName, subscriptionName, cancellationToken).GetAwaiter().GetResult();
            return serviceBusSubscriber;
        }        
    }
}