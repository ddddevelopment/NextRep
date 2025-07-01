using AutoMapper;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Commands.ProcessUpdate;
using TelegramBot.Domain.Models;
using TelegramBot.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBot.Infrastructure.Services;

public class TelegramPollingBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<TelegramPollingBackgroundService> _logger;
    private readonly TelegramBotSettings _settings;

    public TelegramPollingBackgroundService(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IMapper mapper,
        IOptions<TelegramBotSettings> options,
        ILogger<TelegramPollingBackgroundService> logger)
    {
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _settings = options.Value;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!string.IsNullOrEmpty(_settings.WebhookUrl))
        {
            _logger.LogInformation("WebhookUrl configured, polling background service will not start");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Starting Telegram polling background service (WebhookUrl is empty)");

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: stoppingToken);

        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var mapped = _mapper.Map<TelegramUpdate>(update);
            await mediator.Send(new ProcessUpdateCommand { Update = mapped }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing update {UpdateId}", update.Id);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram polling error");
        return Task.CompletedTask;
    }
} 