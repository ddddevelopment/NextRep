using TelegramBot.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace TelegramBot.Infrastructure.Services;

public class TelegramUserService : ITelegramUserService
{
    private readonly TelegramBotDbContext _context;
    private readonly ILogger<TelegramUserService> _logger;

    public TelegramUserService(TelegramBotDbContext context, ILogger<TelegramUserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<TelegramUser>> GetOrCreateTelegramUserAsync(
        long telegramId, 
        string? firstName, 
        string? lastName, 
        string? username)
    {
        try
        {
            var user = await _context.TelegramUsers
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (user == null)
            {
                user = new TelegramUser
                {
                    TelegramId = telegramId,
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    State = UserState.Initial,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.TelegramUsers.Add(user);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Created new Telegram user: {TelegramId}", telegramId);
            }
            else
            {
                // Обновляем информацию о пользователе, если она изменилась
                var updated = false;
                
                if (user.FirstName != firstName)
                {
                    user.FirstName = firstName;
                    updated = true;
                }
                
                if (user.LastName != lastName)
                {
                    user.LastName = lastName;
                    updated = true;
                }
                
                if (user.Username != username)
                {
                    user.Username = username;
                    updated = true;
                }

                if (updated)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            return Result<TelegramUser>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting or creating Telegram user: {TelegramId}", telegramId);
            return Result<TelegramUser>.Failure("Ошибка работы с пользователем");
        }
    }

    public async Task<Result<TelegramUser>> UpdateUserStateAsync(
        long telegramId, 
        UserState state, 
        string? temporaryData = null)
    {
        try
        {
            var user = await _context.TelegramUsers
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (user == null)
                return Result<TelegramUser>.Failure("Пользователь не найден");

            user.State = state;
            user.TemporaryData = temporaryData;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogDebug("Updated user state: {TelegramId} -> {State}", telegramId, state);

            return Result<TelegramUser>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user state: {TelegramId}", telegramId);
            return Result<TelegramUser>.Failure("Ошибка обновления состояния пользователя");
        }
    }

    public async Task<Result<TelegramUser>> LinkUserAccountAsync(long telegramId, Guid userId)
    {
        try
        {
            var user = await _context.TelegramUsers
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (user == null)
                return Result<TelegramUser>.Failure("Пользователь не найден");

            user.UserId = userId;
            user.State = UserState.Authenticated;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Linked Telegram user {TelegramId} with user account {UserId}", 
                telegramId, userId);

            return Result<TelegramUser>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking user account: {TelegramId} -> {UserId}", telegramId, userId);
            return Result<TelegramUser>.Failure("Ошибка связывания аккаунта");
        }
    }
}