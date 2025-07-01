using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Logout;

public class LogoutCommandHandler : BaseTelegramCommandHandler<LogoutCommand>
{
    private readonly IUserSessionService _userSessionService;

    public LogoutCommandHandler(
        ITelegramBotService botService,
        IUserSessionService userSessionService,
        ILogger<LogoutCommandHandler>? logger = null)
        : base(botService, logger)
    {
        _userSessionService = userSessionService;
    }

    protected override async Task<Result> ExecuteAsync(LogoutCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;

        try
        {
            // Проверяем, был ли пользователь авторизован
            var isAuthenticated = await _userSessionService.IsUserAuthenticatedAsync(chatId, cancellationToken);
            
            if (!isAuthenticated)
            {
                var notAuthMessage = "ℹ️ *Вы уже не авторизованы*\n\n" +
                                    "Для входа в систему используйте:\n" +
                                    "• /login - Вход в систему\n" +
                                    "• /register - Регистрация нового аккаунта";

                return await BotService.SendMessageAsync(chatId, notAuthMessage, cancellationToken);
            }

            // Удаляем токен авторизации и информацию о пользователе
            await _userSessionService.RemoveAuthTokenAsync(chatId, cancellationToken);
            
            Logger?.LogInformation("User logged out from chat: {ChatId}", chatId);

            var successMessage = "✅ *Вы успешно вышли из системы*\n\n" +
                                "Ваша сессия завершена. Для использования функций, требующих авторизации, " +
                                "вам необходимо войти в систему заново.\n\n" +
                                "💡 *Доступные команды:*\n" +
                                "• /login - Вход в систему\n" +
                                "• /register - Регистрация\n" +
                                "• /help - Справка\n" +
                                "• /start - Главная информация\n\n" +
                                "Спасибо за использование NextRep! 👋";

            return await BotService.SendMessageAsync(chatId, successMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Unexpected error during logout for chat: {ChatId}", chatId);
            
            var errorMessage = "❌ *Произошла ошибка при выходе*\n\n" +
                              "Попробуйте позже или обратитесь к администратору";

            return await BotService.SendMessageAsync(chatId, errorMessage, cancellationToken);
        }
    }
} 