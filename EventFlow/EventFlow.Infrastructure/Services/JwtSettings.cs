using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Services
{
    public class JwtSettings
    {
        public string SecretKey { get; set;  } = string.Empty;
        public string Issuer {  get; set; } = string.Empty;
        public string Audience {  get; set; } = string.Empty;
        public int AccessTokenLifetimeMinutes { get; set; } = 15;
        public int RefreshTokenLifetimeDays { get; set; } = 30;

    }
}
