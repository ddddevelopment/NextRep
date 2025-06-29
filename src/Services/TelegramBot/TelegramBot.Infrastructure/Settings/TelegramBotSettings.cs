namespace TelegramBot.Infrastructure.Settings;

public class TelegramBotSettings {
    public const string SectionName = "TelegramBot";
    
    public string Token { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
} 