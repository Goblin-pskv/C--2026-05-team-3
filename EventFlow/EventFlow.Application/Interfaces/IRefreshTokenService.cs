using EventFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId);
        Task<User?> ValidateRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken, Guid userId);
    }
}
