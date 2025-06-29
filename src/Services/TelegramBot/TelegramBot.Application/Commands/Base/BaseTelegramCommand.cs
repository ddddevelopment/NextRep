using MediatR;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands.Base;

public abstract class BaseTelegramCommand : IRequest<Result> {
    public TelegramMessage Message { get; set; } = null!;
} 