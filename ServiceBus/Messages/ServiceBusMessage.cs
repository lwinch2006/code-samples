using System;
using System.Linq;

namespace ServiceBusMessages
{
    public class ServiceBusMessage<T>
        where T : class
    {
        public Metadata Metadata { get; init; } = default!;
        public T Payload { get; init; } = default!;
        public Guid? CorrelationId { get; init; }  
        public string? SessionId { get; init; }
        public string? ReplyToSessionId { get; init; }

        public static ServiceBusMessage<T>? FromObject(object? source)
        {
            if (source == null)
            {
                return null;
            }

            var sourceType = source.GetType();
            var destinationType = typeof(ServiceBusMessage<T>);
            
            if ($"{sourceType.Namespace}.{sourceType.Name}" != $"{destinationType.Namespace}.{destinationType.Name}")
            {
                throw new InvalidCastException(
                    $"Unable to cast object of type '{sourceType.FullName}' to type '{destinationType.FullName}'");
            }

            var destination = new ServiceBusMessage<T>();
            var destinationPublicProperties = destination.GetType().GetProperties();
            var sourcePublicProperties = sourceType.GetProperties();

            foreach (var sourcePublicProperty in sourcePublicProperties)
            {
                var value = sourcePublicProperty.GetValue(source, null);
                var destinationPublicProperty = destinationPublicProperties.Single(t => t.Name == sourcePublicProperty.Name);

                if (sourcePublicProperty.PropertyType.IsGenericParameter)
                {
                    destinationPublicProperty.SetValue(destination, (T) value!);
                    continue;
                }
                
                destinationPublicProperty.SetValue(destination, value);
            }
            
            return destination;
        }
    }
}