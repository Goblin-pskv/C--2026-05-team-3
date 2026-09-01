using EventFlow.Domain.Entities;

namespace EventFlow.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId);
        Task<User?> ValidateRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken, Guid userId);
    }
}
