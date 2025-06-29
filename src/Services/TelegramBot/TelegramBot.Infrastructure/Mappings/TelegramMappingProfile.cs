using AutoMapper;
using Telegram.Bot.Types;
using TelegramBot.Domain.Models;

namespace TelegramBot.Infrastructure.Mappings;

public class TelegramMappingProfile : Profile {
    public TelegramMappingProfile() {
        CreateMap<Update, TelegramUpdate>()
            .ForMember(dest => dest.UpdateId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));

        CreateMap<Message, TelegramMessage>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Chat.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));

        CreateMap<User, TelegramUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.IsBot, opt => opt.MapFrom(src => src.IsBot));
    }
} 