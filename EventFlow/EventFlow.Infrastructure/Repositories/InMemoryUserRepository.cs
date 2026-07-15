using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private static readonly List<User> _users = new();
        public Task AddAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByEmailAsync(string email) => await Task.FromResult(_users.Any(u => u.Email == email));

        public async Task<User?> GetByEmailAsync(string email) => await Task.FromResult(_users.FirstOrDefault(u => u.Email == email));

        public async Task<User?> GetByIdAsync(Guid id) => await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

        public async Task SaveChangesAsync() => await Task.CompletedTask;


        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}