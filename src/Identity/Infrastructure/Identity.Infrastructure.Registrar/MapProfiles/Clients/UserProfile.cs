using AutoMapper;
using Identity.Contracts.Clients.Users;
using Identity.Contracts.Contexts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Registrar.MapProfiles.Clients
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserLoginClientRequest, UserLoginRequest>();
            CreateMap<UserRegisterClientRequest, UserRegisterRequest>();
            CreateMap<UserUpdateClientRequest, UserUpdateRequest>();
            CreateMap<UserSummary, UserSummaryClientResponse>();
            CreateMap<UserDetails, UserDetailsClientResponse>();
            CreateMap<UserGenerateEmailTokenClientRequest, UserGenerateEmailTokenRequest>();
            CreateMap<UserChangeEmailClientRequest, UserChangeEmailRequest>();
            CreateMap<UserEmailConfirmClientRequest, UserEmailConfirmRequest>();
            CreateMap<UserGenerateEmailConfirmationTokenClientRequest, UserGenerateEmailConfirmationTokenRequest>();
            
        }

    }
}
