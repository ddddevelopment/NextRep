using MediatR;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Help;
using TelegramBot.Application.Commands.Login;
using TelegramBot.Application.Commands.Logout;
using TelegramBot.Application.Commands.Menu;
using TelegramBot.Application.Commands.Profile;
using TelegramBot.Application.Commands.Register;
using TelegramBot.Application.Commands.Start;
using TelegramBot.Application.Commands.Stats;
using TelegramBot.Application.Commands.Unknown;
using TelegramBot.Application.Commands.Workouts;
using TelegramBot.Application.Commands.CreateExerciseInfo;
using System.Text.RegularExpressions;
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
        var fullText = message.Text?.Trim() ?? "";
        var parts = fullText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts.Length > 0 ? parts[0].ToLower() : "";
        
        _logger?.LogInformation("Routing command: {Command} for user {UserId}", command, message.From.Id);

        return command switch {
            "/start" => await _mediator.Send(new StartCommand { Message = message }, cancellationToken),
            "/help" => await _mediator.Send(new HelpCommand { Message = message }, cancellationToken),
            "/menu" => await _mediator.Send(new MenuCommand { Message = message }, cancellationToken),
            "/workouts" => await _mediator.Send(new WorkoutsCommand { Message = message }, cancellationToken),
            "/profile" => await _mediator.Send(new ProfileCommand { Message = message }, cancellationToken),
            "/stats" => await _mediator.Send(new StatsCommand { Message = message }, cancellationToken),
            "/logout" => await _mediator.Send(new LogoutCommand { Message = message }, cancellationToken),
            "/login" => await HandleLoginCommand(message, parts, cancellationToken),
            "/register" => await HandleRegisterCommand(message, parts, cancellationToken),
            "/createexerciseinfo" => await HandleCreateExerciseInfoCommand(message, cancellationToken),
            _ => await _mediator.Send(new UnknownCommand { Message = message }, cancellationToken)
        };
    }

    private async Task<Result> HandleLoginCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken) {
        if (parts.Length < 3) {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                             "Используйте: `/login email password`\n\n" +
                             "*Пример:*\n" +
                             "`/login user@example.com mypassword`\n\n" +
                             "⚠️ *Внимание:* Рекомендуется удалить сообщение с паролем после входа в систему.";
            
            var helpMsg = new TelegramMessage
            {
                MessageId = message.MessageId,
                Text = helpMessage,
                From = message.From,
                ChatId = message.ChatId,
                Date = message.Date
            };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var email = parts[1];
        var password = parts[2];

        return await _mediator.Send(new LoginCommand 
        { 
            Message = message, 
            Email = email, 
            Password = password 
        }, cancellationToken);
    }

    private async Task<Result> HandleRegisterCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken) {
        if (parts.Length < 6) {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                             "Используйте: `/register email password firstName lastName telephone`\n\n" +
                             "*Пример:*\n" +
                             "`/register user@example.com mypassword Иван Петров +79991234567`\n\n" +
                             "⚠️ *Внимание:* Рекомендуется удалить сообщение с паролем после регистрации.";
            
            var helpMsg = new TelegramMessage
            {
                MessageId = message.MessageId,
                Text = helpMessage,
                From = message.From,
                ChatId = message.ChatId,
                Date = message.Date
            };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var email = parts[1];
        var password = parts[2];
        var firstName = parts[3];
        var lastName = parts[4];
        var telephone = parts[5];

        return await _mediator.Send(new RegisterCommand 
        { 
            Message = message, 
            Email = email, 
            Password = password, 
            FirstName = firstName, 
            LastName = lastName,
            Telephone = telephone
        }, cancellationToken);
    }

    private async Task<Result> HandleCreateExerciseInfoCommand(TelegramMessage message, CancellationToken cancellationToken)
    {
        var args = Regex.Matches(message.Text ?? "", @"[\""].+?[\""]|[^ ]+")
            .Cast<Match>()
            .Select(m => m.Value.Trim('\"'))
            .ToList();

        if (args.Count < 3)
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/createexerciseinfo \"Название\" \"Группа мышц\" \"Описание (опционально)\"`\n\n" +
                              "▶️ *Группы мышц (на английском):* `Chest`, `Back`, `Shoulders`, `Biceps`, `Triceps`, `Legs`, `Core`\n\n" +
                              "*Пример:*\n" +
                              "`/createexerciseinfo \"Жим лежа\" \"Chest\" \"Классическое упражнение на грудные мышцы\"`";
            
            var helpMsg = new TelegramMessage
            {
                MessageId = message.MessageId,
                Text = helpMessage,
                From = message.From,
                ChatId = message.ChatId,
                Date = message.Date
            };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var command = new CreateExerciseInfoCommand
        {
            Message = message,
            Name = args[1],
            MuscleGroup = args[2],
            Description = args.Count > 3 ? args[3] : null
        };

        return await _mediator.Send(command, cancellationToken);
    }
} 