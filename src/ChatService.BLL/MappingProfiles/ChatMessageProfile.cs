using AutoMapper;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.DAL.Models;

namespace ChatService.BLL.MappingProfiles;

public class ChatMessageProfile : Profile
{
    public ChatMessageProfile()
    {
        CreateMap<ChatMessageRequest, ChatMessage>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

        CreateMap<ChatMessage, ChatMessageResponse>()
            .ConstructUsing(
                src => new ChatMessageResponse(
                    src.Id,
                    src.ChatId,
                    src.SenderId,
                    src.Content,
                    src.Timestamp));
    }
}
