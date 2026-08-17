using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using RefreshToken = EventFlow.Domain.Entities.RefreshToken;

namespace EventFlow.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly EventFlowDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenService(EventFlowDbContext context,
                                   UserManager<User> userManager,
                                   JwtSettings jwtSettings)
        {
            _context = context;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Создает и сохраняет RefreshToken
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber); //заполняем массив randomNumber случайными байтами
            var tokenString = Convert.ToBase64String(randomNumber); // токен

            // хэшируем перед отправкой в БД
            string hashToken = BCrypt.Net.BCrypt.HashPassword(tokenString);

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = hashToken, // в БД отправляем хэш
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();


            return tokenString;
        }
        public async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
            {
                return null;
            }

            return storedToken.User;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, Guid userId)
        {
            var storedToken = await _context
                .RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

    }
}
