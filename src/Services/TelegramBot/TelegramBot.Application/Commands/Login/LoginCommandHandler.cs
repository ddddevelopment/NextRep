using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Login;

public class LoginCommandHandler : BaseTelegramCommandHandler<LoginCommand>
{
    private readonly IAuthApiClient _authApiClient;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IUserSessionService _userSessionService;

    public LoginCommandHandler(
        ITelegramBotService botService,
        IAuthApiClient authApiClient,
        IUsersApiClient usersApiClient,
        IUserSessionService userSessionService,
        ILogger<LoginCommandHandler>? logger = null)
        : base(botService, logger)
    {
        _authApiClient = authApiClient;
        _usersApiClient = usersApiClient;
        _userSessionService = userSessionService;
    }

    protected override async Task<Result> ExecuteAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;

        try
        {
            Logger?.LogInformation("Attempting login for user: {Email} from chat: {ChatId}", request.Email, chatId);

            // Авторизация через Auth API
            var loginResult = await _authApiClient.LoginAsync(new LoginDto
            {
                Email = request.Email,
                Password = request.Password
            }, cancellationToken);

            if (!loginResult.IsSuccess)
            {
                var errorMessage = "❌ *Ошибка авторизации*\n\n" +
                                  $"Причина: {loginResult.Error?.Message ?? "Неверный email или пароль"}";

                return await BotService.SendMessageAsync(chatId, errorMessage, cancellationToken);
            }

            var authResponse = loginResult.Value!;

            // Сохраняем токен авторизации
            await _userSessionService.SetAuthTokenAsync(chatId, authResponse.AccessToken, cancellationToken);

            // Получаем информацию о пользователе
            var userResult = await _usersApiClient.GetByEmailAsync(request.Email, cancellationToken);
            
            if (userResult.IsSuccess && userResult.Value != null)
            {
                await _userSessionService.SetUserInfoAsync(chatId, userResult.Value, cancellationToken);
            }

            var successMessage = "✅ *Успешная авторизация!*\n\n" +
                                $"Добро пожаловать, {userResult.Value?.Name ?? "пользователь"}!\n\n" +
                                "Теперь вы можете использовать все функции бота:\n" +
                                "• 💪 /workouts - Управление тренировками\n" +
                                "• 👤 /profile - Профиль пользователя\n" +
                                "• 📊 /stats - Статистика";

            return await BotService.SendMessageAsync(chatId, successMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Unexpected error during login for user: {Email} from chat: {ChatId}", request.Email, chatId);
            
            var errorMessage = "❌ *Произошла неожиданная ошибка*\n\n" +
                              "Попробуйте позже или обратитесь к администратору";

            return await BotService.SendMessageAsync(chatId, errorMessage, cancellationToken);
        }
    }
} 