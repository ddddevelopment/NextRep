using TelegramBot.Application.Commands;
using MediatR;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthTokenDto>>
    {
        private readonly IUserService _userService;
        private readonly ITelegramUserService _telegramUserService;

        public LoginUserCommandHandler(
            IUserService userService,
            ITelegramUserService telegramUserService)
        {
            _userService = userService;
            _telegramUserService = telegramUserService;
        }

        public async Task<Result<AuthTokenDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var loginDto = new LoginDto {
                Email = request.Email,
                Password = request.Password
            };
            var authResult = await _userService.LoginAsync(loginDto);

            if (!authResult.IsSuccess)
                return authResult;

            // Обновляем состояние пользователя на аутентифицированный
            await _telegramUserService.UpdateUserStateAsync(
                request.TelegramId, 
                UserState.Authenticated);

            return authResult;
        }
    }
}