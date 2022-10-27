using System;

namespace ServiceBusMessages
{
    public class Metadata
    {
        public string EventType { get; init; } = default!;
        public string EventName { get; init; } = default!;
        public int Version { get; init; }
        public string CreatedBy { get; init; } = default!;
        public DateTime CreatedOnUtc { get; init; }
    }
}