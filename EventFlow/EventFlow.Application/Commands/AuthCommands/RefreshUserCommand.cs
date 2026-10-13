using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Commands.AuthCommands
{
    public record RefreshUserCommand(string refresh) : IRequest<Result<AuthResponseDto>>;
}
