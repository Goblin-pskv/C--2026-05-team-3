using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Queries.LoginQuery
{
    public record LoginQuery
    (
        string Email,
        string Password
        ) : IRequest<Result<AuthResponseDto>>;
}
