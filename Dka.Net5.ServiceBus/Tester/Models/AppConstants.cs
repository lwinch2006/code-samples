namespace ServiceBusTester.Models
{
    public static class AppConstants
    {
        public static class Events
        {
            public static class TenantEvents
            {
                public const string Type = "TenantEvents";

                public static class TenantUpdated
                {
                    public const string Name = "TenantUpdated";
                    public static readonly string FullName = $"{Type}.{Name}";
                }
            }

            public static class UserEvents
            {
                public const string Type = "UserEvents";

                public static class UserUpdated
                {
                    public const string Name = "UserUpdated";
                    public static readonly string FullName = $"{Type}.{Name}";
                }
            }

            public static class Responses
            {
                public const string Type = "Responses";
                
                public static class GeneralResponse
                {
                    public const string Name = "GeneralResponse";
                    public static readonly string FullName = $"{Type}.{Name}";
                }
            }
        }
        
        public static class ServiceBus
        {
            public static class Publish
            {
                public const string Queue1 = "tenantevents";
                public const string RequestQueue1 = "request-queue1";
                
                public static readonly string Topic1 = "events";

                public static readonly string[] Queues = { Queue1, RequestQueue1 };
                public static readonly string[] Topics = { Topic1 };
            }
            
            public static class Receive
            {
                public const string Queue1 = "tenantevents";

                public const string ResponseQueue1 = "response-queue1"; 
                
                public const string Topic1 = "events";
                public const string TopicSubscription1 = "servicebustester";
                public const string TopicSubscription2 = "servicebustester2";
                
                public static readonly string[] Queues = { Queue1, ResponseQueue1 };
                public static readonly string[] Topics = { Topic1 };
                public static readonly (string, string)[] TopicSubscriptions = 
                {
                    (Topic1, TopicSubscription1),
                    (Topic1, TopicSubscription2)
                };
            }
        }
    }
}