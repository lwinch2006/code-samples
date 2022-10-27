using System;

namespace ServiceBusTester.Models.Events.V1.User
{
    public class UserUpdated
    {
        public Guid Id { get; init; }
        public string NewName { get; init; } = default!;        
    }
}