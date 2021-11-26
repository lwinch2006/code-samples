namespace ServiceBusSubscriber;

public enum ServiceBusSubscriberReceiveMessageTypes
{
    // just T in ServiceBusMessage<T>
    Payload = 1,
    
    // ServiceBusMessage<T>
    Message = 2,
    
    // ServiceBusReceivedMessage (From Azure library)
    FullMessage = 3
}