using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly EventFlowDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenService(EventFlowDbContext context, UserManager<User> userManager, JwtSettings jwtSettings)
        {
            _context = context;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        public Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
        public Task<User?> ValidateRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task RevokeRefreshTokenAsync(string refreshToken, Guid userId)
        {
            throw new NotImplementedException();
        }

    }
}
