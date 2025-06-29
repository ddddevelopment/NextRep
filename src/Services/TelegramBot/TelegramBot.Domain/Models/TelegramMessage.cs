namespace TelegramBot.Domain.Models;

public class TelegramMessage {
    public int MessageId { get; set; }
    public TelegramUser From { get; set; } = null!;
    public long ChatId { get; set; }
    public string? Text { get; set; }
    public DateTime Date { get; set; }
} 