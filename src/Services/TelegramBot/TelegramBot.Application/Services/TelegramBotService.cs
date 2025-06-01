using MediatR;
using TelegramBot.Application.Commands;
using TelegramBot.Application.Queries;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Services;

public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramUserService _telegramUserService;
        private readonly IMediator _mediator;

        public TelegramBotService(
            ITelegramUserService telegramUserService,
            IMediator mediator)
        {
            _telegramUserService = telegramUserService;
            _mediator = mediator;
        }

        public async Task<Result<string>> ProcessUpdateAsync(ProcessTelegramUpdateCommand command)
        {
            var userResult = await _telegramUserService.GetOrCreateTelegramUserAsync(
                command.TelegramId,
                command.FirstName,
                command.LastName,
                command.Username);

            if (!userResult.IsSuccess)
                return Result<string>.Failure("Ошибка получения пользователя");

            var user = userResult.Value!;
            var messageText = command.MessageText.Trim();

            return user.State switch
            {
                UserState.Initial => await HandleInitialState(user, messageText),
                UserState.WaitingForRegistration => await HandleRegistrationState(user, messageText),
                UserState.WaitingForLogin => await HandleLoginState(user, messageText),
                UserState.Authenticated => await HandleAuthenticatedState(user, messageText),
                // UserState.AddingWorkout => await HandleAddingWorkoutState(user, messageText),
                UserState.WaitingForWorkoutName => await HandleWorkoutNameState(user, messageText),
                UserState.WaitingForExerciseData => await HandleExerciseDataState(user, messageText),
                _ => Result<string>.Success("🤖 Неизвестная команда. Введите /help для справки.")
            };
        }

        private async Task<Result<string>> HandleInitialState(TelegramUser user, string messageText)
        {
            return messageText.ToLower() switch
            {
                "/start" => Result<string>.Success(
                    "🏋️‍♂️ Добро пожаловать в GymTracker!\n\n" +
                    "Для начала работы выберите действие:\n" +
                    "/register - Регистрация нового аккаунта\n" +
                    "/login - Войти в существующий аккаунт\n" +
                    "/help - Помощь"),

                "/register" => await StartRegistration(user),
                "/login" => await StartLogin(user),
                "/help" => Result<string>.Success(GetHelpText()),
                _ => Result<string>.Success("Введите /start для начала работы")
            };
        }

        private async Task<Result<string>> HandleAuthenticatedState(TelegramUser user, string messageText)
        {
            return messageText.ToLower() switch
            {
                "/workout" => await StartWorkoutCreation(user),
                "/workouts" => await ShowUserWorkouts(user),
                "/stats" => await ShowUserStats(user),
                "/help" => Result<string>.Success(GetAuthenticatedHelpText()),
                _ => Result<string>.Success("Неизвестная команда. Введите /help для справки.")
            };
        }

        private async Task<Result<string>> StartRegistration(TelegramUser user)
        {
            await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.WaitingForRegistration);
            return Result<string>.Success(
                "📝 Регистрация\n\n" +
                "Отправьте данные в формате:\n" +
                "email,password,firstName,lastName\n\n" +
                "Пример: john@example.com,mypassword123,John,Doe");
        }

        private async Task<Result<string>> StartLogin(TelegramUser user)
        {
            await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.WaitingForLogin);
            return Result<string>.Success(
                "🔐 Вход в систему\n\n" +
                "Отправьте данные в формате:\n" +
                "email,password\n\n" +
                "Пример: john@example.com,mypassword123");
        }

        private async Task<Result<string>> HandleRegistrationState(TelegramUser user, string messageText)
        {
            var parts = messageText.Split(',');
            if (parts.Length != 4)
            {
                return Result<string>.Success(
                    "❌ Неверный формат данных.\n" +
                    "Используйте: email,password,firstName,lastName");
            }

            var command = new RegisterUserCommand(
                user.TelegramId,
                parts[0].Trim(),
                parts[1].Trim(),
                parts[2].Trim(),
                parts[3].Trim());

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.Authenticated);
                return Result<string>.Success(
                    "✅ Регистрация успешно завершена!\n\n" +
                    $"Добро пожаловать, {result.Value!.FirstName}!\n\n" +
                    "Доступные команды:\n" +
                    "/workout - Добавить тренировку\n" +
                    "/workouts - Мои тренировки\n" +
                    "/stats - Статистика\n" +
                    "/help - Помощь");
            }

            await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.Initial);
            return Result<string>.Success($"❌ Ошибка регистрации: {result.Error}");
        }

        private async Task<Result<string>> HandleLoginState(TelegramUser user, string messageText)
        {
            var parts = messageText.Split(',');
            if (parts.Length != 2)
            {
                return Result<string>.Success(
                    "❌ Неверный формат данных.\n" +
                    "Используйте: email,password");
            }

            var command = new LoginUserCommand(
                user.TelegramId,
                parts[0].Trim(),
                parts[1].Trim());

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.Authenticated);
                return Result<string>.Success(
                    "✅ Вход выполнен успешно!\n\n" +
                    "Доступные команды:\n" +
                    "/workout - Добавить тренировку\n" +
                    "/workouts - Мои тренировки\n" +
                    "/stats - Статистика\n" +
                    "/help - Помощь");
            }

            await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.Initial);
            return Result<string>.Success($"❌ Ошибка входа: {result.Error}");
        }

        private async Task<Result<string>> StartWorkoutCreation(TelegramUser user)
        {
            await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.WaitingForWorkoutName);
            return Result<string>.Success(
                "🏋️‍♂️ Создание тренировки\n\n" +
                "Введите название тренировки:");
        }

        private async Task<Result<string>> HandleWorkoutNameState(TelegramUser user, string messageText)
        {
            await _telegramUserService.UpdateUserStateAsync(
                user.TelegramId, 
                UserState.WaitingForExerciseData, 
                messageText);

            return Result<string>.Success(
                $"📝 Тренировка: {messageText}\n\n" +
                "Теперь добавьте упражнения в формате:\n" +
                "НазваниеУпражнения:вес1xповторы1,вес2xповторы2;СледующееУпражнение:...\n\n" +
                "Пример:\n" +
                "Жим лежа:100x8,105x6,110x4;Приседания:120x10,130x8\n\n" +
                "Отправьте /done когда закончите");
        }

        private async Task<Result<string>> HandleExerciseDataState(TelegramUser user, string messageText)
        {
            if (messageText.ToLower() == "/done")
            {
                // Создаем тренировку из временных данных
                var workoutResult = await CreateWorkoutFromData(user);
                await _telegramUserService.UpdateUserStateAsync(user.TelegramId, UserState.Authenticated);
                return workoutResult;
            }

            // Сохраняем данные упражнений
            var existingData = user.TemporaryData ?? "";
            var updatedData = string.IsNullOrEmpty(existingData) ? messageText : $"{existingData}|{messageText}";
            
            await _telegramUserService.UpdateUserStateAsync(
                user.TelegramId, 
                UserState.WaitingForExerciseData, 
                updatedData);

            return Result<string>.Success(
                "✅ Упражнения добавлены!\n\n" +
                "Добавьте еще упражнения или отправьте /done для завершения");
        }

        private async Task<Result<string>> CreateWorkoutFromData(TelegramUser user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.TemporaryData))
                    return Result<string>.Success("❌ Нет данных для создания тренировки");

                var parts = user.TemporaryData.Split('|');
                var workoutName = parts.Length > 1 ? "Моя тренировка" : parts[0];
                var exerciseData = parts.Length > 1 ? string.Join("|", parts.Skip(1)) : parts[0];

                var exercises = ParseExercises(exerciseData);
                
                var createWorkoutDto = new CreateWorkoutDto(
                    workoutName,
                    DateTime.UtcNow,
                    exercises);

                var command = new CreateWorkoutCommand(user.TelegramId, createWorkoutDto);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Result<string>.Success(
                        $"✅ Тренировка '{result.Value!.Name}' успешно создана!\n\n" +
                        $"📅 Дата: {result.Value.Date:dd.MM.yyyy}\n" +
                        $"🏋️‍♂️ Упражнений: {result.Value.Exercises.Count}");
                }

                return Result<string>.Success($"❌ Ошибка создания тренировки: {result.Error}");
            }
            catch (Exception ex)
            {
                return Result<string>.Success($"❌ Ошибка обработки данных: {ex.Message}");
            }
        }

        private List<CreateExerciseDto> ParseExercises(string exerciseData)
        {
            var exercises = new List<CreateExerciseDto>();
            
            foreach (var exerciseBlock in exerciseData.Split('|'))
            {
                var exerciseInfos = exerciseBlock.Split(';');
                
                foreach (var exerciseInfo in exerciseInfos)
                {
                    if (string.IsNullOrWhiteSpace(exerciseInfo)) continue;
                    
                    var exerciseParts = exerciseInfo.Split(':');
                    if (exerciseParts.Length != 2) continue;
                    
                    var exerciseName = exerciseParts[0].Trim();
                    var sets = ParseSets(exerciseParts[1]);
                    
                    if (sets.Any())
                    {
                        exercises.Add(new CreateExerciseDto(exerciseName, sets));
                    }
                }
            }
            
            return exercises;
        }
        
        private List<CreateSetDto> ParseSets(string setsData)
        {
            var sets = new List<CreateSetDto>();
            
            foreach (var setInfo in setsData.Split(','))
            {
                var trimmedSet = setInfo.Trim();
                if (string.IsNullOrEmpty(trimmedSet)) continue;
                
                var setParts = trimmedSet.Split('x');
                if (setParts.Length == 2 && 
                    decimal.TryParse(setParts[0], out var weight) && 
                    int.TryParse(setParts[1], out var reps))
                {
                    sets.Add(new CreateSetDto(reps, weight));
                }
            }
            
            return sets;
        }

        private async Task<Result<string>> ShowUserWorkouts(TelegramUser user)
        {
            var query = new GetUserWorkoutsQuery(user.TelegramId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return Result<string>.Success($"❌ Ошибка получения тренировок: {result.Error}");

            var workouts = result.Value!;
            if (!workouts.Any())
                return Result<string>.Success("📋 У вас пока нет тренировок.\n\nИспользуйте /workout для создания первой тренировки!");

            var response = "📋 Ваши тренировки:\n\n";
            
            foreach (var workout in workouts.Take(5))
            {
                response += $"🏋️‍♂️ {workout.Name}\n";
                response += $"📅 {workout.Date:dd.MM.yyyy}\n";
                response += $"💪 Упражнений: {workout.Exercises.Count}\n";
                
                foreach (var exercise in workout.Exercises.Take(3))
                {
                    var maxWeight = exercise.Sets.Max(s => s.Weight);
                    var totalSets = exercise.Sets.Count;
                    response += $"   • {exercise.Name}: {maxWeight}кг x{totalSets} подходов\n";
                }
                
                if (workout.Exercises.Count > 3)
                    response += $"   ... и еще {workout.Exercises.Count - 3} упражнений\n";
                    
                response += "\n";
            }

            if (workouts.Count > 5)
                response += $"... и еще {workouts.Count - 5} тренировок";

            return Result<string>.Success(response);
        }

        private async Task<Result<string>> ShowUserStats(TelegramUser user)
        {
            var query = new GetWorkoutStatsQuery(user.TelegramId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return Result<string>.Success($"❌ Ошибка получения статистики: {result.Error}");

            var stats = result.Value!;
            
            var response = "📊 Ваша статистика:\n\n";
            response += $"🏋️‍♂️ Всего тренировок: {stats.TotalWorkouts}\n\n";
            
            if (stats.MaxWeights.Any())
            {
                response += "💪 Максимальные веса:\n";
                foreach (var maxWeight in stats.MaxWeights.Take(5))
                {
                    response += $"   • {maxWeight.Key}: {maxWeight.Value}кг\n";
                }
                response += "\n";
            }
            
            if (stats.Progress.Any())
            {
                response += "📈 Последний прогресс:\n";
                var recentProgress = stats.Progress
                    .OrderByDescending(p => p.Date)
                    .Take(5);
                    
                foreach (var progress in recentProgress)
                {
                    response += $"   • {progress.ExerciseName}: {progress.MaxWeight}кг ({progress.Date:dd.MM})\n";
                }
            }

            return Result<string>.Success(response);
        }

        private string GetHelpText()
        {
            return "🤖 Справка GymTracker\n\n" +
                   "Доступные команды:\n" +
                   "/start - Начать работу\n" +
                   "/register - Регистрация\n" +
                   "/login - Вход в систему\n" +
                   "/help - Эта справка\n\n" +
                   "Для работы с приложением необходимо зарегистрироваться или войти в существующий аккаунт.";
        }

        private string GetAuthenticatedHelpText()
        {
            return "🤖 Справка GymTracker\n\n" +
                   "Доступные команды:\n" +
                   "/workout - Добавить новую тренировку\n" +
                   "/workouts - Просмотр ваших тренировок\n" +
                   "/stats - Статистика и прогресс\n" +
                   "/help - Эта справка\n\n" +
                   "💡 Советы:\n" +
                   "• При добавлении тренировки используйте формат: НазваниеУпражнения:весxповторы\n" +
                   "• Разделяйте подходы запятыми, упражнения - точкой с запятой\n" +
                   "• Следите за своим прогрессом в разделе статистики!";
        }
    }