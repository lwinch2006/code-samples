namespace Application.Models
{
    public static class ApplicationConstants
    {
        public static class Authentication
        {
            public static class Claims
            {
                public const string Amr = "amr";
            }
        }

        public static class ServiceBus
        {
            public static class Configuration
            {
                public const string ConnnectionStringPath = "ServiceBus:ConnectionString";
            }
            
            public static class Publish
            {
                public const string SendEventsTopic = "events";

                public static readonly string[] Queues = { };
                public static readonly string[] Topics = { SendEventsTopic };
            }
            
            public static class Receive
            {
                public static readonly string[] Queues = { };
                public static readonly string[] Topics = { };
                public static readonly (string, string)[] TopicSubscriptions = { };
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
}