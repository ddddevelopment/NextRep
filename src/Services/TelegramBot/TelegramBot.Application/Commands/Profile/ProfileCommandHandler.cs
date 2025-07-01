using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Profile;

public class ProfileCommandHandler : BaseTelegramCommandHandler<ProfileCommand> {
    private readonly IUserSessionService _userSessionService;

    public ProfileCommandHandler(
        ITelegramBotService botService, 
        IUserSessionService userSessionService,
        ILogger<ProfileCommandHandler>? logger = null) 
        : base(botService, logger) {
        _userSessionService = userSessionService;
    }

    protected override async Task<Result> ExecuteAsync(ProfileCommand request, CancellationToken cancellationToken) {
        var chatId = request.Message.ChatId;
        
        // Проверяем авторизацию пользователя
        var isAuthenticated = await _userSessionService.IsUserAuthenticatedAsync(chatId, cancellationToken);
        if (!isAuthenticated)
        {
            var notAuthMessage = "🚫 *Для просмотра профиля требуется авторизация*\n\n" +
                                "Пожалуйста, войдите в систему через команду /login или зарегистрируйтесь через /register";
            
            return await BotService.SendMessageAsync(chatId, notAuthMessage, cancellationToken);
        }

        // Получаем информацию о пользователе из сессии
        var userInfo = await _userSessionService.GetUserInfoAsync(chatId, cancellationToken);
        
        string profileMessage;
        
        if (userInfo != null)
        {
            var id = EscapeMarkdownV2(userInfo.Id.ToString());
            var email = EscapeMarkdownV2(userInfo.Email);
            var name = EscapeMarkdownV2(userInfo.Name);
            var phone = EscapeMarkdownV2(userInfo.Telephone ?? "не указан");
            var telegramId = EscapeMarkdownV2(request.Message.From.Id.ToString());
            var telegramUsername = string.IsNullOrEmpty(request.Message.From.Username) 
                ? "не указан" 
                : "@" + EscapeMarkdownV2(request.Message.From.Username);

            profileMessage = $"👤 *Профиль пользователя*\n\n" +
                           $"🆔 *ID:* {id}\n" +
                           $"📧 *Email:* {email}\n" +
                           $"👨‍💻 *Имя:* {name}\n" +
                           $"📞 *Телефон:* {phone}\n\n" +
                           "📱 *Telegram:*\n" +
                           $"• ID: {telegramId}\n" +
                           $"• Username: {telegramUsername}\n\n" +
                           "⚙️ *Настройки:*\n" +
                           "• Единицы измерения: кг/см\n" +
                           "• Уведомления: включены\n" +
                           "• Режим: новичок\n\n" +
                           "🎯 *Цели:*\n" +
                           "• Тренировок в неделю: не установлено\n" +
                           "• Целевой вес: не установлен\n\n" +
                           "🔐 *Управление аккаунтом:*\n" +
                           "• /logout \\- Выйти из аккаунта";
        }
        else
        {
            // Fallback если информация о пользователе не найдена
            profileMessage = $"👤 *Профиль пользователя*\n\n" +
                           $"🆔 *Telegram ID:* {request.Message.From.Id}\n" +
                           $"👨‍💻 *Telegram имя:* {request.Message.From.FirstName}" +
                           (string.IsNullOrEmpty(request.Message.From.LastName) ? "" : $" {request.Message.From.LastName}") + "\n" +
                           $"📝 *Username:* {(string.IsNullOrEmpty(request.Message.From.Username) ? "не указан" : "@" + EscapeMarkdownV2(request.Message.From.Username))}\n\n" +
                           "⚠️ *Информация о профиле недоступна*\n" +
                           "Попробуйте войти в систему заново через /login\n\n" +
                           "🔐 *Управление аккаунтом:*\n" +
                           "• /logout \\- Выйти из аккаунта";
        }

        return await BotService.SendMessageAsync(chatId, profileMessage, cancellationToken);
    }
    
    private string EscapeMarkdownV2(string text)
    {
        var specialChars = new[] { "_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
        foreach (var c in specialChars)
        {
            text = text.Replace(c, $"\\{c}");
        }
        return text;
    }
} 