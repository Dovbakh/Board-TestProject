using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Application.AppData.Repositories;
using Identity.Contracts.Contexts.Users;
using Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IdentityServer4.Models.IdentityResources;

namespace Identity.Infrastructure.DataAccess.Contexts.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ILogger<UserRepository> _logger;
        private const string RegisterEmailKey = "RegisterEmailKey_";

        public UserRepository(UserManager<Domain.User> userManager, IMapper mapper, RoleManager<IdentityRole<Guid>> roleManager, IDistributedLockFactory distributedLockFactory, 
            ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _distributedLockFactory = distributedLockFactory;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<UserSummary>> GetAllAsync(int offset, int count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение списка пользователей. Смещение: {1}, количество: {2}",
                nameof(GetAllAsync), offset, count);

            var users = await _userManager.Users
                .ProjectTo<UserSummary>(_mapper.ConfigurationProvider)
                .Skip(offset).Take(count).ToListAsync(cancellation);

            return users;
        }

        public async Task<UserDetails> GetByEmailAsync(string email, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение пользователя с email: {1}",
                nameof(GetByEmailAsync), email);

            var user = await _userManager.Users
                .Where(u => u.Email == email)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return user;
        }

        public async Task<UserDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение пользователя с ID: {1}",
                nameof(GetByIdAsync), id);

            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .ProjectTo<UserDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return user;
        }

        public async Task<Guid> AddAsync(UserRegisterRequest registerRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Добавления нового пользователя из модели: {1}",
                nameof(AddAsync), JsonConvert.SerializeObject(registerRequest));

            var newEntity = _mapper.Map<UserRegisterRequest, Domain.User>(registerRequest);

            var resource = RegisterEmailKey + registerRequest.Email;
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    var result = await _userManager.CreateAsync(newEntity, registerRequest.Password);

                    if (!result.Succeeded)
                    {
                        throw new ArgumentException($"Ошибка при регистрации нового пользователя: {JsonConvert.SerializeObject(result.Errors)}");
                    }
                }
            }

            return newEntity.Id;
        }


        public async Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление информации о пользователе с ID: {1} из модели {2}: {3}",
                nameof(UpdateAsync), id, nameof(UserUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с ID: {id} не найден.");
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

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new ArgumentException($"При обновлении информации о пользователе произошла ошибка:: {JsonConvert.SerializeObject(result.Errors)}");
            }

            return _mapper.Map<User, UserDetails>(user);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление пользователя с ID: {1}",
                nameof(DeleteAsync), id);

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с ID: {id} не найден.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new ArgumentException($"При удалении пользователя произошла ошибка:: {JsonConvert.SerializeObject(result.Errors)}");
            }
        }

        public async Task ChangeEmailAsync(string currentEmail, string newEmail, string token, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Изменения email пользователя с {1} на {2}",
                nameof(ConfirmEmailAsync), currentEmail, newEmail);

            var user = await _userManager.FindByEmailAsync(currentEmail);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {currentEmail}");
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            if (!result.Succeeded)
            {
                throw new ArgumentException($"При изменении почты произошла ошибка:: {JsonConvert.SerializeObject(result.Errors)}");
            }

            await _userManager.SetUserNameAsync(user, newEmail);
        }

        public async Task ConfirmEmailAsync(string email, string token, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Подтверждение email пользователя: {1}",
                nameof(ConfirmEmailAsync), email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {email}");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new ArgumentException($"При подтверждении почты произошла ошибка:: {JsonConvert.SerializeObject(result.Errors)}");
            }
        }

        public async Task<EmailChangeToken> GenerateEmailTokenAsync(string currentEmail, string newEmail, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Генерация токена изменения email с {1} на {2}",
                nameof(GenerateEmailTokenAsync), currentEmail, newEmail);

            var user = await _userManager.FindByEmailAsync(currentEmail);
            if(user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {currentEmail}");
            }

            var tokenValue = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            return new EmailChangeToken { Value = tokenValue };
        }

        public async Task<EmailConfirmationToken> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Генерация токена подтверждения email: {1}",
                nameof(GenerateEmailConfirmationTokenAsync), email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {email}");
            }

            var tokenValue = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return new EmailConfirmationToken { Value = tokenValue };
        }

        public async Task ChangePasswordAsync(string email, string currentPassword, string newPassword, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Изменение пароля пользователя с email: {1}",
                nameof(ChangePasswordAsync), email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {email}");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                throw new ArgumentException($"При изменения пароля произошла ошибка:: {JsonConvert.SerializeObject(result.Errors)}");
            }
        }

        public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Валидация пароля пользователя с email: {1}",
                nameof(ChangePasswordAsync), email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {email}");
            }

            return await _userManager.CheckPasswordAsync(user, password);
        }


        public async Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Сброс пароля пользователя с email: {1}",
                nameof(ResetPasswordAsync), email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {email}");
            }

            token = token.Replace(" ", "+");
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                throw new ArgumentException($"При сбросе пароля произошла ошибка:: {JsonConvert.SerializeObject(result.Errors)}");
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Генерация токена для сброса пароля пользователя с email: {1}",
                nameof(GeneratePasswordResetTokenAsync), email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким email не найден: {email}");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return token;
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение информации имеет ли пользователь с ID: {1} роль {2}",
                nameof(IsInRoleAsync), userId, role);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с таким ID не найден: {userId}");
            }

            var hasRole = await _userManager.IsInRoleAsync(user, role);

            return hasRole;
        }
    }
}
