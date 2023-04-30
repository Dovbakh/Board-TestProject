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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserLoginRequest, UserLoginClientRequest>();
            CreateMap<UserRegisterRequest, UserRegisterClientRequest>();
            CreateMap<UserUpdateRequest, UserUpdateClientRequest>();
            CreateMap<UserSummaryClientResponse, UserSummary>();
            CreateMap<UserDetailsClientResponse, UserDetails>();
            CreateMap<UserGenerateEmailTokenRequest, UserGenerateEmailTokenClientRequest>();
            CreateMap<UserGenerateEmailConfirmationTokenRequest, UserGenerateEmailConfirmationTokenClientRequest>();
            
        }
    }
}
