using System;

namespace ServiceBusMessages
{
    public class Metadata
    {
        public string EventType { get; set; }
        public string EventName { get; set; }
        public int Version { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}