namespace TelegramBot.Domain.Models;

public class Error {
    public ErrorType Type { get; set; }
    public string? Message { get; set; }

    public Error(ErrorType type, string? message = null)
    {
        Type = type;
        Message = message;
    }
} 