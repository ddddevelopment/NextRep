using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.Login;

public class LoginCommand : BaseTelegramCommand
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
} 