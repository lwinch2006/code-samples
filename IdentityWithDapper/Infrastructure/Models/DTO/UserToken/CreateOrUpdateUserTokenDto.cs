﻿using System;

namespace IdentityWithDapper.Infrastructure.Models.DTO.UserToken
{
    public class CreateOrUpdateUserTokenDto
    {
        public Guid UserId { get; set; }
        public string  LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }          
    }
}