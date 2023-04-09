using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Users.Repositories;
using Board.Contracts.Contexts.Users;
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
        private readonly IMapper _mapper;

        public UserRepository(UserManager<Domain.User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<UserSummary>> GetAll(int offset, int count, CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .ProjectTo<UserSummary>(_mapper.ConfigurationProvider)
                .Skip(offset).Take(count).ToListAsync();
        }

        public async Task<UserDetails> GetByEmail(string email, CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .Where(u => u.Email == email)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<UserDetails> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .Where(u => u.Id == id)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> AddAsync(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            var newEntity = _mapper.Map<UserRegisterRequest, Domain.User>(registerRequest);

            await _userManager.CreateAsync(newEntity, registerRequest.Password);
            return newEntity.Id;
        }

        public Task ChangeEmailAsync(string currentEmail, string newEmail, string token, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> ChangeEmailRequestAsync(string currentEmail, string newEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ChangePasswordAsync(string email, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            await _userManager.DeleteAsync(user);
        }



        public Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> ResetPasswordRequestAsync(string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            
            //if (!updateRequest.Name.IsNullOrEmpty())
            if(!string.IsNullOrEmpty(updateRequest.Name))
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
        }
    }
}
