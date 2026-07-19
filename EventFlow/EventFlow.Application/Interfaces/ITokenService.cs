using EventFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
