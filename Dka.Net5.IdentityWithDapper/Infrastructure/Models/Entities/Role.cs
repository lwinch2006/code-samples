﻿using System;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities
{
    public class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}