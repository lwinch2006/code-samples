using System;
using System.Collections.Generic;

namespace ServiceBusSubscriber
{
    public class ServiceBusSubscriberConfiguration
    {
        public Dictionary<(int, string), Type> DeserializationDictionary { get; set; }
    }
}