using MediatR;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.ProcessUpdate;

public class ProcessUpdateCommandHandler : IRequestHandler<ProcessUpdateCommand, Result> {
    private readonly ICommandRouter _commandRouter;
    private readonly ILogger<ProcessUpdateCommandHandler>? _logger;

    public ProcessUpdateCommandHandler(ICommandRouter commandRouter, ILogger<ProcessUpdateCommandHandler>? logger = null) {
        _commandRouter = commandRouter;
        _logger = logger;
    }

    public async Task<Result> Handle(ProcessUpdateCommand request, CancellationToken cancellationToken) {
        _logger?.LogInformation("Processing update: {UpdateId}", request.Update.UpdateId);

        try {
            if (request.Update.Message?.Text != null) {
                return await _commandRouter.RouteCommandAsync(request.Update.Message, cancellationToken);
            }

            _logger?.LogDebug("Update {UpdateId} doesn't contain text message", request.Update.UpdateId);
            return Result.Success();
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Error processing update {UpdateId}", request.Update.UpdateId);
            return Result.Failure($"Error processing update: {ex.Message}");
        }
    }
} 