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
                    public const string Name = "UserUpdated";
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
        }
    }
}