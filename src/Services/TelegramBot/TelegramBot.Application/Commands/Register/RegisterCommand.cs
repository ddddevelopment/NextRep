using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.Register;

public class RegisterCommand : BaseTelegramCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
} 