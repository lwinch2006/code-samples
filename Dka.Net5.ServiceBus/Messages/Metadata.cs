using System;

namespace ServiceBusMessages
{
    public class Metadata
    {
        public string EventType { get; set; }
        public string EventName { get; set; }
        public DateTime Timestamp { get; set; }
        public int Version { get; set; }
    }
}