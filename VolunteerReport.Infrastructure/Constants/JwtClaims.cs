﻿using Microsoft.IdentityModel.JsonWebTokens;

namespace EnRoute.Infrastructure.Constants
{
    public class JwtClaims
    {
        public const string Sub = JwtRegisteredClaimNames.Sub;
        public const string Email = JwtRegisteredClaimNames.Email;
        public const string Name = JwtRegisteredClaimNames.Name;
        public const string AvatarUrl = "avatarUrl";
        public const string RegisterDate = "registerDate";
        public const string Roles = "roles";
    }
}
