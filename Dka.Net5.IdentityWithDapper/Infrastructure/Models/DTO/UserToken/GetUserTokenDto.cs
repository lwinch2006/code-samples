﻿using System;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserToken
{
    public class GetUserTokenDto
    {
        public Guid UserId { get; set; }
        public string  LoginProvider { get; set; }
        public string Name { get; set; }        
    }
}