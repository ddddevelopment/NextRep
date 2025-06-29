using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramBot.Application.Commands.ProcessUpdate;
using TelegramBot.Domain.Models;

namespace TelegramBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase {
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<WebhookController>? _logger;

    public WebhookController(IMediator mediator, IMapper mapper, ILogger<WebhookController>? logger = null) {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, CancellationToken cancellationToken) {
        _logger?.LogInformation("Received webhook update: {UpdateId}", update.Id);

        try {
            var telegramUpdate = _mapper.Map<TelegramUpdate>(update);
            var command = new ProcessUpdateCommand { Update = telegramUpdate };
            
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess) {
                _logger?.LogInformation("Update {UpdateId} processed successfully", update.Id);
                return Ok();
            }
            else {
                _logger?.LogWarning("Failed to process update {UpdateId}: {Error}", update.Id, result.Error?.Message);
                return Ok(); // Возвращаем OK чтобы Telegram не повторял запрос
            }
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Error processing webhook update {UpdateId}", update.Id);
            return Ok(); // Возвращаем OK чтобы Telegram не повторял запрос
        }
    }
} 