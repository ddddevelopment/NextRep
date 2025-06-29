using MediatR;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands.ProcessUpdate;

public class ProcessUpdateCommand : IRequest<Result> {
    public TelegramUpdate Update { get; set; } = null!;
} 