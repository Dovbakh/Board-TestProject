using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Users.Repositories;
using Board.Contracts.Contexts.Users;
using Board.Contracts.Contexts.Users.Enums;
using Board.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<Domain.User> _userManager;
        private readonly RoleManager<Domain.Role> _roleManager;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<Domain.User> userManager, IMapper mapper, RoleManager<Domain.Role> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<IReadOnlyCollection<UserSummary>> GetAll(int offset, int count, CancellationToken cancellation)
        {
            return await _userManager.Users
                .ProjectTo<UserSummary>(_mapper.ConfigurationProvider)
                .Skip(offset).Take(count).ToListAsync(cancellation);
        }

        public async Task<UserDetails> GetByEmail(string email, CancellationToken cancellation)
        {
            return await _userManager.Users
                .Where(u => u.Email == email)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);
        }

        public async Task<UserDetails> GetById(Guid id, CancellationToken cancellation)
        {
            return await _userManager.Users
                .Where(u => u.Id == id)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);
        }

        public async Task<Guid> AddAsync(UserRegisterRequest registerRequest, CancellationToken cancellation)
        {
            var newEntity = _mapper.Map<UserRegisterRequest, Domain.User>(registerRequest);
            var result = await _userManager.CreateAsync(newEntity, registerRequest.Password);
            if(!result.Succeeded) 
            { 
                throw new ArgumentException(result.Errors.ToList().ToString());
            }

            await _userManager.AddToRoleAsync(newEntity, Contracts.Contexts.Users.Enums.Role.User);

            return newEntity.Id;
        }


        public async Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            //if (!updateRequest.Name.IsNullOrEmpty())
            if (!string.IsNullOrEmpty(updateRequest.Name))
            {
                user.Name = updateRequest.Name;
            }
            if (!string.IsNullOrEmpty(updateRequest.Phone))
            {
                user.PhoneNumber = updateRequest.Phone;
            }
            if (!string.IsNullOrEmpty(updateRequest.Address))
            {
                user.Address = updateRequest.Address;
            }

            await _userManager.UpdateAsync(user);

            return _mapper.Map<User, UserDetails>(user);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            await _userManager.DeleteAsync(user);
        }

        public Task ChangeEmailAsync(string currentEmail, string newEmail, string token, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<string> ChangeEmailRequestAsync(string currentEmail, string newEmail, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task ChangePasswordAsync(string email, string currentPassword, string newPassword, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }


        public Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<string> ResetPasswordRequestAsync(string email, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }


    }
}
