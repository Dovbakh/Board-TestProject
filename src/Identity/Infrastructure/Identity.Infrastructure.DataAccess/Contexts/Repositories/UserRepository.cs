using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Application.AppData.Repositories;
using Identity.Contracts.Contexts.Users;
using Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.DataAccess.Contexts.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<Domain.User> userManager, IMapper mapper, RoleManager<IdentityRole<Guid>> roleManager)
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
            var user = await _userManager.Users
                .Where(u => u.Email == email)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return user;
        }

        public async Task<UserDetails> GetById(Guid id, CancellationToken cancellation)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return user;
        }

        public async Task<Guid> AddAsync(UserRegisterRequest registerRequest, CancellationToken cancellation)
        {
            var newEntity = _mapper.Map<UserRegisterRequest, Domain.User>(registerRequest);
            var result = await _userManager.CreateAsync(newEntity, registerRequest.Password);
            if(!result.Succeeded) 
            { 
                throw new ArgumentException(result.Errors.ToList().ToString());
            }

            return newEntity.Id;
        }


        public async Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync(cancellation);

            if (user == null)
            {
                throw new KeyNotFoundException();
            }


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

        public async Task ChangeEmailAsync(string currentEmail, string newEmail, string token, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(currentEmail);

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            if (!result.Succeeded)
            {
                throw new ArgumentException(string.Join("~", result.Errors));
            }

            await _userManager.SetUserNameAsync(user, newEmail);
        }

        public async Task ConfirmEmailAsync(string email, string token, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new ArgumentException(string.Join("~", result.Errors));
            }

        }

        public async Task<EmailChangeToken> GenerateEmailTokenAsync(string currentEmail, string newEmail, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(currentEmail);

            var tokenValue = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            return new EmailChangeToken { Value = tokenValue };
        }

        public async Task<EmailConfirmationToken> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var tokenValue = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return new EmailConfirmationToken { Value = tokenValue };
        }

        public async Task ChangePasswordAsync(string email, string currentPassword, string newPassword, CancellationToken cancellation)
        {
            var user = await _userManager.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join("~", result.Errors));
            }
        }

        public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellation)
        {
            var user = await _userManager.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            return await _userManager.CheckPasswordAsync(user, password);

        }


        public async Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(email);

            token = token.Replace(" ", "+");
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join("~", result.Errors));
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return token;
        }

        public async Task<bool> IsInRoleRole(Guid userId, string role, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            var hasRole = await _userManager.IsInRoleAsync(user, role);

            return hasRole;
        }

    }
}
