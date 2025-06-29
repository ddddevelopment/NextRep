using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Profile;

public class ProfileCommandHandler : BaseTelegramCommandHandler<ProfileCommand> {
    public ProfileCommandHandler(ITelegramBotService botService, ILogger<ProfileCommandHandler>? logger = null) 
        : base(botService, logger) {
    }

    protected override async Task<Result> ExecuteAsync(ProfileCommand request, CancellationToken cancellationToken) {
        var profileMessage = $"👤 *Профиль пользователя*\n\n" +
                           $"🆔 *ID:* {request.Message.From.Id}\n" +
                           $"👨‍💻 *Имя:* {request.Message.From.FirstName}" +
                           (string.IsNullOrEmpty(request.Message.From.LastName) ? "" : $" {request.Message.From.LastName}") + "\n" +
                           $"📝 *Username:* {(string.IsNullOrEmpty(request.Message.From.Username) ? "не указан" : "@" + request.Message.From.Username)}\n\n" +
                           "⚙️ *Настройки:*\n" +
                           "• Единицы измерения: кг/см\n" +
                           "• Уведомления: включены\n" +
                           "• Режим: новичок\n\n" +
                           "🎯 *Цели:*\n" +
                           "• Тренировок в неделю: не установлено\n" +
                           "• Целевой вес: не установлен\n\n" +
                           "_Настройки профиля будут расширены в следующих версиях_";

        return await BotService.SendMessageAsync(request.Message.ChatId, profileMessage, cancellationToken);
    }
} 