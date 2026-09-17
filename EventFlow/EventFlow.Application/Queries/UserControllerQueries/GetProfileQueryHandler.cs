using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Queries.GetProfileQuery
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<UserDto>>
    {
        private readonly IUserRepository _repository;

        public GetProfileQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<UserDto>> Handle(GetProfileQuery request, CancellationToken ct)
        {
            var user = await _repository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<UserDto>.Failure("Пользователь не найден", 404);
            //нашёл Dto, но нет конструктора, поэтому тут ЗАГЛУШКА
            var dto = new UserDto();
            dto.FullName = user.FullName;
            dto.FirstName = user.FirstName;
            dto.LastName = user.LastName;
            dto.Email = user.Email;
            dto.PhoneNumber = user.PhoneNumber;
            return Result<UserDto>.Success(dto);
        }
    }
}