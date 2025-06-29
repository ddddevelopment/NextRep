namespace TelegramBot.Domain.Models;

public class TelegramUpdate {
    public int UpdateId { get; set; }
    public TelegramMessage? Message { get; set; }
} 