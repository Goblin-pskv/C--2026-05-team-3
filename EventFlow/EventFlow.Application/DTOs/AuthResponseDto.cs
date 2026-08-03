using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    public record AuthResponseDto(string accessToken,string refreshToken,DateTime expiresAt);
}