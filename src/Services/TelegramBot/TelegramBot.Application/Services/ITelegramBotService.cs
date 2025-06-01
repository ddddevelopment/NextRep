using TelegramBot.Application.Commands;
using TelegramBot.Domain.Models;

public interface ITelegramBotService
{
    Task<Result<string>> ProcessUpdateAsync(ProcessTelegramUpdateCommand command);
}