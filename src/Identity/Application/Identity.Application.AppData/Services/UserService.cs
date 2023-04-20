using Identity.Application.AppData.Repositories;
using Identity.Contracts.Contexts.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.AppData.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;


        public UserService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public Task<IReadOnlyCollection<UserSummary>> GetAll(int? offset, int? count, CancellationToken cancellationToken)
        {

            if (count == 0)
            {
                count = 10;
            }
            return _userRepository.GetAll(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellationToken);
        }

        public Task<UserDetails> GetById(Guid id, CancellationToken cancellationToken)
        {
            return _userRepository.GetById(id, cancellationToken);
        }

        public async Task<UserDetails> GetCurrent(CancellationToken cancellationToken)
        {


            throw new NotImplementedException();
            //_logger.LogInformation("Получение данных о текущем пользователе и их обновление.");

            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(claimId))
            {
                return null;
            }
            var id = Guid.Parse(claimId);

            var user = await _userRepository.GetById(id, cancellationToken);

            return user;

        }

        public async Task<string> Login(UserLoginRequest loginRequest, CancellationToken cancellationToken)
        {
            //var validationResult = _validatorLogin.Validate(loginRequest);
            //if (!validationResult.IsValid)
            //{
            //    throw new Exception(validationResult.ToString("~"));
            //}
            
            
            //await _boardClient.Login();


            var existingUser = await _userRepository.GetByEmail(loginRequest.Email, cancellationToken);
            if (existingUser == null)
            {
                throw new ArgumentException("Введен неверный email или пароль.");
            }

            var isPasswordVerified = await _userRepository.CheckPasswordAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
            if (!isPasswordVerified)
            {
                throw new ArgumentException("Введен неверный email или пароль.");
            }

            var token = "";// GenerateToken(existingUser);


            return token;
        }

        public async Task<Guid> Register(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            //var validationResult = _validatorRegister.Validate(userRegisterDto);
            //if (!validationResult.IsValid)
            //{
            //    throw new Exception(validationResult.ToString("~"));
            //}

            var existingUser = await _userRepository.GetByEmail(registerRequest.Email, cancellationToken);
            if (existingUser != null)
            {

                throw new ArgumentException($"Пользователь с почтой '{registerRequest.Email}' уже зарегистрирован!");
            }


            return await _userRepository.AddAsync(registerRequest, cancellationToken);
        }

        public Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            //var validationResult = _validatorUpdate.Validate(request);
            //if (!validationResult.IsValid)
            //{
            //    throw new Exception(validationResult.ToString("~"));
            //}

            return _userRepository.UpdateAsync(id, updateRequest, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            //var currentUser = await GetCurrent(cancellationToken);
            //;
            //if (currentUser.Id != id && currentUser.RoleName != "admin")
            //{
            //    throw new AccessViolationException("Нет прав.");
            //}

            await _userRepository.DeleteAsync(id, cancellationToken);
        }



        //private string GenerateToken(UserDetails user)
        //{
        //    //_logger.LogInformation("Генерация JWT-токена для пользователя с идентификатором {UserId}", user.Id);

        //    try
        //    {
        //        var claims = new List<Claim> {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Email, user.Email),
        //        new Claim(ClaimTypes.Role, user.RoleId.ToString())
        //    };

        //        var secretKey = _configuration["AuthToken:SecretKey"];

        //        var token = new JwtSecurityToken
        //            (
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddDays(1),
        //            notBefore: DateTime.UtcNow,
        //            signingCredentials: new SigningCredentials(
        //                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        //                SecurityAlgorithms.HmacSha256
        //                )
        //            );

        //        var result = new JwtSecurityTokenHandler().WriteToken(token);

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        //_logger.LogError("Ошибка при генерации токена для пользователя с идентификатором {UserId}: {ErrorMessage}", user.Id, e.Message);
        //        throw;
        //    }
        //}
    }
}
