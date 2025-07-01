using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Application.Commands.Menu;

public class MenuCommandHandler : BaseTelegramCommandHandler<MenuCommand> {
    public MenuCommandHandler(ITelegramBotService botService, ILogger<MenuCommandHandler>? logger = null) 
        : base(botService, logger) {
    }

    protected override async Task<Result> ExecuteAsync(MenuCommand request, CancellationToken cancellationToken) {
        var menuMessage = "📋 *Главное меню NextRep*\n\nВыберите действие нажатием кнопки:";

        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("💪 Тренировки", "/workouts"),
                InlineKeyboardButton.WithCallbackData("📊 Статистика", "/stats")
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData("👤 Профиль", "/profile"),
                InlineKeyboardButton.WithCallbackData("❓ Помощь", "/help")
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData("🔐 Войти", "/login"),
                InlineKeyboardButton.WithCallbackData("�� Выйти", "/logout")
            }
        });

        return await BotService.SendMessageAsync(request.Message.ChatId, menuMessage, keyboard, cancellationToken);
    }
} 