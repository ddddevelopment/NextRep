using MediatR;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Help;
using TelegramBot.Application.Commands.Menu;
using TelegramBot.Application.Commands.Profile;
using TelegramBot.Application.Commands.Start;
using TelegramBot.Application.Commands.Stats;
using TelegramBot.Application.Commands.Unknown;
using TelegramBot.Application.Commands.Workouts;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Services;

public class CommandRouter : ICommandRouter {
    private readonly IMediator _mediator;
    private readonly ILogger<CommandRouter>? _logger;

    public CommandRouter(IMediator mediator, ILogger<CommandRouter>? logger = null) {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result> RouteCommandAsync(TelegramMessage message, CancellationToken cancellationToken = default) {
        var command = message.Text?.Trim().ToLower();
        
        _logger?.LogInformation("Routing command: {Command} for user {UserId}", command, message.From.Id);

        return command switch {
            "/start" => await _mediator.Send(new StartCommand { Message = message }, cancellationToken),
            "/help" => await _mediator.Send(new HelpCommand { Message = message }, cancellationToken),
            "/menu" => await _mediator.Send(new MenuCommand { Message = message }, cancellationToken),
            "/workouts" => await _mediator.Send(new WorkoutsCommand { Message = message }, cancellationToken),
            "/profile" => await _mediator.Send(new ProfileCommand { Message = message }, cancellationToken),
            "/stats" => await _mediator.Send(new StatsCommand { Message = message }, cancellationToken),
            _ => await _mediator.Send(new UnknownCommand { Message = message }, cancellationToken)
        };
    }
} 