using MediatR;
using TelegramBot.Application.Commands;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Handlers
{
    public class ProcessTelegramUpdateCommandHandler : IRequestHandler<ProcessTelegramUpdateCommand, Result<string>>
    {
        private readonly ITelegramBotService _telegramBotService;

        public ProcessTelegramUpdateCommandHandler(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public async Task<Result<string>> Handle(ProcessTelegramUpdateCommand request, CancellationToken cancellationToken)
        {
            return await _telegramBotService.ProcessUpdateAsync(request);
        }
    }
}