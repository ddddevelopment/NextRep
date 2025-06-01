using MediatR;
using Microsoft.AspNetCore.Mvc;
using TelegramBot.Application.Commands;

namespace TelegramBot.Api.Controllers;

[ApiController]
[Route("api/telegram/webhook")]
public class TelegramWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public TelegramWebhookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TelegramUpdateDto update)
    {
        // Преобразуем update в ProcessTelegramUpdateCommand
        var command = new ProcessTelegramUpdateCommand(
            update.TelegramId,
            update.FirstName,
            update.LastName,
            update.Username,
            update.MessageText,
            update.MessageId
        );
        var result = await _mediator.Send(command);
        // Telegram ожидает 200 OK даже если есть ошибка
        return Ok();
    }
}

// DTO для входящего update (можно расширить под нужды Telegram)
public record TelegramUpdateDto(
    long TelegramId,
    string MessageText,
    string? FirstName,
    string? LastName,
    string? Username,
    int MessageId
);
