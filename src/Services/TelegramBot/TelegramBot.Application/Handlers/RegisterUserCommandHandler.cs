using TelegramBot.Application.Commands;
using MediatR;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
    {
        private readonly IUserService _userService;
        private readonly ITelegramUserService _telegramUserService;

        public RegisterUserCommandHandler(
            IUserService userService,
            ITelegramUserService telegramUserService)
        {
            _userService = userService;
            _telegramUserService = telegramUserService;
        }

        public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var registerDto = new RegisterUserDto
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var registerResult = await _userService.RegisterAsync(registerDto);

            if (!registerResult.IsSuccess)
                return registerResult;

            // Связываем Telegram аккаунт с пользователем
            await _telegramUserService.LinkUserAccountAsync(request.TelegramId, registerResult.Value!.Id);

            return registerResult;
        }
    }
}