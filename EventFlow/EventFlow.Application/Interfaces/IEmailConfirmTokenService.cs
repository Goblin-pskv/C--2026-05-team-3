using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    public interface IEmailConfirmTokenService
    {
        Task<string> GenerateAsync(Guid userId, CancellationToken ct);
        Task<bool> ConfirmEmail(Guid userId, string token, CancellationToken ct);
    }
}