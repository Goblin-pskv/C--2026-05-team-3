using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Queries.GetProfileQuery
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetProfileQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<UserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {//В методах нужно будет дописать в параметры токен, как только изменят методы
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<UserDto>.Failure("Пользователь не найден");
            //нашёл Dto, но нет конструктора, поэтому тут ЗАГЛУШКА
            //var dto = new UserDto(
            //    user.Id,
            //    user.FirstName,
            //    user.LastName,
            //    user.Email,
            //    user.PhoneNumber,
            //    user.FullName,
            //    user.Role
            //);
            var dto = new UserDto();
            return Result<UserDto>.Success(dto);
        }
    }
}