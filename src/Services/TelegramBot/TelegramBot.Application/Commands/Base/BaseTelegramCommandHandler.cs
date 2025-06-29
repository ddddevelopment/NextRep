using MediatR;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Base;

public abstract class BaseTelegramCommandHandler<TCommand> : IRequestHandler<TCommand, Result> 
    where TCommand : BaseTelegramCommand {
    
    protected readonly ITelegramBotService BotService;
    protected readonly ILogger? Logger;

    protected BaseTelegramCommandHandler(ITelegramBotService botService, ILogger? logger = null) {
        BotService = botService;
        Logger = logger;
    }

    public async Task<Result> Handle(TCommand request, CancellationToken cancellationToken) {
        Logger?.LogInformation("Handling {CommandType} for user {UserId}", 
            typeof(TCommand).Name, request.Message.From.Id);

        try {
            return await ExecuteAsync(request, cancellationToken);
        }
        catch (Exception ex) {
            Logger?.LogError(ex, "Error handling {CommandType} for user {UserId}", 
                typeof(TCommand).Name, request.Message.From.Id);
            return Result.Failure($"Error handling command: {ex.Message}");
        }
    }

    protected abstract Task<Result> ExecuteAsync(TCommand request, CancellationToken cancellationToken);
} 