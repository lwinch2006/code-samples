using System;

namespace ServiceBusTester.Models.Events.V1.User
{
    public class UserUpdated
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }        
    }
}