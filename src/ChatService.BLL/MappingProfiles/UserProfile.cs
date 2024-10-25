using AutoMapper;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.DAL.Models;

namespace ChatService.BLL.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Patronymic, opt => opt.MapFrom(src => src.Patronymic));

        CreateMap<User, UserResponse>()
            .ConstructUsing(
                src => new UserResponse(
                    src.Id,
                    src.Role,
                    src.Email,
                    src.FirstName,
                    src.LastName,
                    src.Patronymic));
    }
}
