namespace ThreeLayersModernEventsSubscriber.Models;

public static class ApplicationConstants
{
    public static class ServiceBus
    {
        public static class Configuration
        {
            public const string ConnnectionStringPath = "ServiceBus:ConnectionString";
        }
            
        public static class Publish
        {
            public static readonly string[] Queues = { };
            public static readonly string[] Topics = { };
        }
            
        public static class Receive
        {
            public const string ReceiveEventsTopic = "events";
            public const string ReceiveEventsTopicSubscription = "threelayersmoderneventssubscriber";
            
            public static readonly string[] Queues = { };
            public static readonly string[] Topics = { ReceiveEventsTopic };
            public static readonly (string, string)[] TopicSubscriptions = { (ReceiveEventsTopic, ReceiveEventsTopicSubscription) };
        }            
            
        public static class Events
        {
            public static class TenantEvents
            {
                public const string TenantUpdated = "TenantUpdated";
            }
        }
    }    
}