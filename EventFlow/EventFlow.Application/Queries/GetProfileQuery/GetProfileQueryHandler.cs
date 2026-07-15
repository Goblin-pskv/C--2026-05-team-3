using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace EventFlow.Application.Queries.GetProfileQuery
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<GetProfileQuery> _validator;

        public GetProfileQueryHandler(IUserRepository userRepository, IValidator<GetProfileQuery> validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }
        public async Task<Result> Handle(GetProfileQuery request, CancellationToken cancellationToken)
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