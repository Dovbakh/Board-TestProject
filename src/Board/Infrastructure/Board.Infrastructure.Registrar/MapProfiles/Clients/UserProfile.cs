using AutoMapper;
using Board.Contracts.Contexts.Users;
using Identity.Contracts.Clients.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles.Clients
{
    /// <summary>
    /// Профиль AutoMapper для работы с User
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserLoginRequest, UserLoginClientRequest>();

            CreateMap<UserRegisterRequest, UserRegisterClientRequest>();

            CreateMap<UserUpdateRequest, UserUpdateClientRequest>();

            CreateMap<UserSummaryClientResponse, UserSummary>();

            CreateMap<UserDetailsClientResponse, UserDetails>()
                .ForMember(c => c.Rating, map => map.Ignore());

            CreateMap<UserGenerateEmailTokenRequest, UserGenerateEmailTokenClientRequest>();

            CreateMap<UserGenerateEmailConfirmationTokenRequest, UserGenerateEmailConfirmationTokenClientRequest>();
            
        }
    }
}
