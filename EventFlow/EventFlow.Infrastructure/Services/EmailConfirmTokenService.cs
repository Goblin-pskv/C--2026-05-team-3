
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Infrastructure.Services
{
    public class EmailConfirmTokenService : IEmailConfirmTokenService
    {
        private readonly UserManager<User> _userManager;

        public EmailConfirmTokenService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GenerateAsync(Guid userId, CancellationToken ct)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: ct)
                ?? throw new InvalidOperationException("Пользователь не найден");
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<bool> ConfirmEmail(Guid userId, string token, CancellationToken ct)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: ct);
            if (user is null || user.EmailConfirmed) return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

    }
}