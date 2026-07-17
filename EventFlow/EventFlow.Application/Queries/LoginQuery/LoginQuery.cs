using EventFlow.Application.Common;
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
        ) : IRequest<Result<string>>;
}
