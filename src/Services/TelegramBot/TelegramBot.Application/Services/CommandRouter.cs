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
using TelegramBot.Application.Commands.GetExerciseInfos;
using TelegramBot.Application.Commands.DeleteExerciseInfo;
using TelegramBot.Application.Commands.UpdateExerciseInfo;
using TelegramBot.Application.Commands.GetExerciseInfoById;
using TelegramBot.Application.Commands.CreateWorkout;
using TelegramBot.Application.Commands.GetWorkoutById;
using TelegramBot.Application.Commands.UpdateWorkout;
using TelegramBot.Application.Commands.DeleteWorkout;
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
            "/create_exercise_info" => await HandleCreateExerciseInfoCommand(message, cancellationToken),
            "/get_all_exercise_infos" => await _mediator.Send(new GetExerciseInfosCommand { Message = message }, cancellationToken),
            "/get_exercise_info_by_id" => await HandleGetExerciseInfoByIdCommand(message, parts, cancellationToken),
            "/delete_exercise_info" => await HandleDeleteExerciseInfoCommand(message, parts, cancellationToken),
            "/update_exercise_info" => await HandleUpdateExerciseInfoCommand(message, cancellationToken),
            "/create_workout" => await HandleCreateWorkoutCommand(message, parts, cancellationToken),
            "/get_workout_by_id" => await HandleGetWorkoutByIdCommand(message, parts, cancellationToken),
            "/update_workout" => await HandleUpdateWorkoutCommand(message, parts, cancellationToken),
            "/delete_workout" => await HandleDeleteWorkoutCommand(message, parts, cancellationToken),
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

    private async Task<Result> HandleGetExerciseInfoByIdCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken)
    {
        if (parts.Length < 2 || !Guid.TryParse(parts[1], out var exerciseInfoId))
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/get_exercise_info_by_id <ID упражнения>`\n\n" +
                              "Вы можете получить ID, вызвав команду /get_all_exercise_infos.";
            
            var helpMsg = new TelegramMessage { MessageId = message.MessageId, Text = helpMessage, From = message.From, ChatId = message.ChatId, Date = message.Date };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var command = new GetExerciseInfoByIdCommand { Message = message, ExerciseInfoId = exerciseInfoId };
        return await _mediator.Send(command, cancellationToken);
    }

    private async Task<Result> HandleDeleteExerciseInfoCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken)
    {
        if (parts.Length < 2 || !Guid.TryParse(parts[1], out var exerciseInfoId))
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/delete_exercise_info <ID упражнения>`\n\n" +
                              "Вы можете получить ID, вызвав команду /get_all_exercise_infos.";
            
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

        var command = new DeleteExerciseInfoCommand
        {
            Message = message,
            ExerciseInfoId = exerciseInfoId
        };

        return await _mediator.Send(command, cancellationToken);
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
                              "Используйте: `/create_exercise_info \"Название\" \"Группа мышц\" \"Описание (опционально)\"`\n\n" +
                              "▶️ *Группы мышц (на английском):* `Chest`, `Back`, `Shoulders`, `Biceps`, `Triceps`, `Legs`, `Core`\n\n" +
                              "*Пример:*\n" +
                              "`/create_exercise_info \"Жим лежа\" \"Chest\" \"Классическое упражнение на грудные мышцы\"`";
            
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

    private async Task<Result> HandleUpdateExerciseInfoCommand(TelegramMessage message, CancellationToken cancellationToken)
    {
        var args = Regex.Matches(message.Text ?? "", @"[\""].+?[\""]|[^ ]+")
            .Cast<Match>()
            .Select(m => m.Value.Trim('\"'))
            .ToList();

        if (args.Count < 4 || !Guid.TryParse(args[1], out var exerciseInfoId))
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/update_exercise_info <ID> \"Новое название\" \"Новая группа мышц\" \"Новое описание (опц.)\"`\n\n" +
                              "▶️ *Группы мышц:* `Chest`, `Back`, `Shoulders`, `Biceps`, `Triceps`, `Legs`, `Core`\n\n" +
                              "*Пример:*\n" +
                              "`/update_exercise_info 1234-abcd \"Жим лежа V2\" \"Chest\"`";
            
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

        var command = new UpdateExerciseInfoCommand
        {
            Message = message,
            ExerciseInfoId = exerciseInfoId,
            Name = args[2],
            MuscleGroup = args[3],
            Description = args.Count > 4 ? args[4] : null
        };

        return await _mediator.Send(command, cancellationToken);
    }

    private async Task<Result> HandleCreateWorkoutCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken)
    {
        var args = Regex.Matches(message.Text ?? "", @"[\""].+?[\""]|[^ ]+")
            .Cast<Match>()
            .Select(m => m.Value.Trim('\"'))
            .ToList();

        if (args.Count < 2)
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/create_workout \"Имя тренировки\" \"Заметки (опционально)\"`\n\n" +
                              "*Пример:*\n" +
                              "`/create_workout \"Утренняя зарядка\"`";

            var helpMsg = new TelegramMessage { MessageId = message.MessageId, Text = helpMessage, From = message.From, ChatId = message.ChatId, Date = message.Date };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var command = new CreateWorkoutCommand
        {
            Message = message,
            Name = args[1],
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Notes = args.Count > 2 ? args[2] : null
        };

        return await _mediator.Send(command, cancellationToken);
    }

    private async Task<Result> HandleGetWorkoutByIdCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken)
    {
        if (parts.Length < 2 || !Guid.TryParse(parts[1], out var workoutId))
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/get_workout_by_id <ID тренировки>`\n\n" +
                              "Вы можете получить ID, вызвав команду /workouts.";
            
            var helpMsg = new TelegramMessage { MessageId = message.MessageId, Text = helpMessage, From = message.From, ChatId = message.ChatId, Date = message.Date };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var command = new GetWorkoutByIdCommand { Message = message, WorkoutId = workoutId };
        return await _mediator.Send(command, cancellationToken);
    }

    private async Task<Result> HandleUpdateWorkoutCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken)
    {
        var args = Regex.Matches(message.Text ?? "", @"[\""].+?[\""]|[^ ]+")
            .Cast<Match>()
            .Select(m => m.Value.Trim('\"'))
            .ToList();

        if (args.Count < 3 || !Guid.TryParse(args[1], out var workoutId))
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/update_workout <ID> \"Новое имя\" \"Новые заметки (опционально)\"`\n\n" +
                              "*Пример:*\n" +
                              "`/update_workout 1234-abcd \"Вечерняя пробежка\" \"Было легко\"`";
            
            var helpMsg = new TelegramMessage { MessageId = message.MessageId, Text = helpMessage, From = message.From, ChatId = message.ChatId, Date = message.Date };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var command = new UpdateWorkoutCommand
        {
            Message = message,
            WorkoutId = workoutId,
            Name = args[2],
            Notes = args.Count > 3 ? args[3] : null
        };
        
        return await _mediator.Send(command, cancellationToken);
    }

    private async Task<Result> HandleDeleteWorkoutCommand(TelegramMessage message, string[] parts, CancellationToken cancellationToken)
    {
        if (parts.Length < 2 || !Guid.TryParse(parts[1], out var workoutId))
        {
            var helpMessage = "❌ *Неверный формат команды*\n\n" +
                              "Используйте: `/delete_workout <ID тренировки>`\n\n" +
                              "Вы можете получить ID, вызвав команду /workouts.";
            
            var helpMsg = new TelegramMessage { MessageId = message.MessageId, Text = helpMessage, From = message.From, ChatId = message.ChatId, Date = message.Date };
            
            await _mediator.Send(new UnknownCommand { Message = helpMsg }, cancellationToken);
            return Result.Success();
        }

        var command = new DeleteWorkoutCommand { Message = message, WorkoutId = workoutId };
        return await _mediator.Send(command, cancellationToken);
    }
} 