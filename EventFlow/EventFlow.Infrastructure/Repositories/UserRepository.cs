using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly EventFlowDbContext _context;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, EventFlowDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IdentityResult> AddAsync(User user, string password)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var createdUserResult = await _userManager.CreateAsync(user, password);
                if(!createdUserResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return createdUserResult;
                }
                var setRoleResult = await _userManager.AddToRoleAsync(user, "User");
                if(!setRoleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return setRoleResult;
                }
                await transaction.CommitAsync();
                return IdentityResult.Success;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
                return true;
            else 
                return false;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> Update(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}