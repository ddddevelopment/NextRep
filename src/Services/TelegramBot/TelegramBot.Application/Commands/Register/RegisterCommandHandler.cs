using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Register;

public class RegisterCommandHandler : BaseTelegramCommandHandler<RegisterCommand>
{
    private readonly IAuthApiClient _authApiClient;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IUserSessionService _userSessionService;

    public RegisterCommandHandler(
        ITelegramBotService botService,
        IAuthApiClient authApiClient,
        IUsersApiClient usersApiClient,
        IUserSessionService userSessionService,
        ILogger<RegisterCommandHandler>? logger = null)
        : base(botService, logger)
    {
        _authApiClient = authApiClient;
        _usersApiClient = usersApiClient;
        _userSessionService = userSessionService;
    }

    protected override async Task<Result> ExecuteAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;

        try
        {
            Logger?.LogInformation("Attempting registration for user: {Email} from chat: {ChatId}", request.Email, chatId);

            // Регистрация через Auth API
            var registerResult = await _authApiClient.RegisterAsync(new RegisterDto
            {
                Email = request.Email,
                Password = request.Password,
                Name = request.FirstName + " " + request.LastName,
                Telephone = request.Telephone
            }, cancellationToken);

            if (!registerResult.IsSuccess)
            {
                var errorMessage = "❌ *Ошибка регистрации*\n\n" +
                                  $"Причина: {registerResult.Error?.Message ?? "Неизвестная ошибка"}";

                return await BotService.SendMessageAsync(chatId, errorMessage, cancellationToken);
            }

            var authResponse = registerResult.Value!;

            // Сохраняем токен авторизации
            await _userSessionService.SetAuthTokenAsync(chatId, authResponse.AccessToken, cancellationToken);

            // Получаем информацию о созданном пользователе
            var userResult = await _usersApiClient.GetByEmailAsync(request.Email, cancellationToken);
            
            if (userResult.IsSuccess && userResult.Value != null)
            {
                await _userSessionService.SetUserInfoAsync(chatId, userResult.Value, cancellationToken);
            }

            var successMessage = "🎉 *Регистрация успешно завершена!*\n\n" +
                                $"Добро пожаловать в NextRep, {request.FirstName}!\n\n" +
                                "Ваш аккаунт создан и вы автоматически авторизованы.\n\n" +
                                "Доступные функции:\n" +
                                "• 💪 /workouts - Управление тренировками\n" +
                                "• 👤 /profile - Профиль пользователя\n" +
                                "• 📊 /stats - Статистика\n" +
                                "• 📋 /menu - Главное меню\n\n" +
                                "Начните свой фитнес-путь прямо сейчас! 🚀";

            return await BotService.SendMessageAsync(chatId, successMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Unexpected error during registration for user: {Email} from chat: {ChatId}", request.Email, chatId);
            
            var errorMessage = "❌ *Произошла неожиданная ошибка*\n\n" +
                              "Попробуйте позже или обратитесь к администратору";

            return await BotService.SendMessageAsync(chatId, errorMessage, cancellationToken);
        }
    }
} 