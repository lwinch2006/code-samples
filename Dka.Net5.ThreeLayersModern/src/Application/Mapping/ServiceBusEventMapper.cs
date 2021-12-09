using ServiceBusMessages;

namespace Application.Mapping;

public static class ServiceBusEventMapper
{
    public static ServiceBusMessage<T> Map<T>(
        string eventType, 
        string eventName, 
        int version, 
        string createdBy, 
        T payload,
        Guid? correlationId = default,
        string sessionId = default,
        string replyToSessionId = default)
        where T : class
    {
        var metadata = new Metadata
        {
            EventType = eventType,
            EventName = eventName,
            Version = version,
            CreatedBy = createdBy,
            CreatedOnUtc = DateTime.UtcNow,
            
        };
        
        var message = new ServiceBusMessage<T>
        {
            CorrelationId = correlationId,
            Metadata = metadata,
            Payload = payload,
            SessionId = sessionId,
            ReplyToSessionId = replyToSessionId
        };

        return message;
    }
}