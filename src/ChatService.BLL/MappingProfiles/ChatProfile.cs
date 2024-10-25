using AutoMapper;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.DAL.Models;

namespace ChatService.BLL.MappingProfiles;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<ChatRequest, Chat>()
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));

        CreateMap<Chat, ChatResponse>()
            .ConstructUsing(
                src => new ChatResponse(
                    src.Id,
                    src.Participants,
                    src.LastModified));
    }
}
