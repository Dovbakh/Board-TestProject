using Identity.Application.AppData.Services;
using Identity.Contracts.Contexts.Users;
using Identity.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Host.Server.Controllers
{
    /// <summary>
    /// Работа с пользователями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous] // TODO: удалить
    [ApiConventionType(typeof(AppConventions))]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Работа с пользователями.
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Получить список всех пользователей с пагинацией.
        /// </summary>
        /// <param name="take">Количество получаемых пользователей.</param>
        /// <param name="skip">Количество пропускаемых пользователей.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="UserSummary"/>.</returns>
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IReadOnlyCollection<UserSummary>>> GetAll(int offset, int count, CancellationToken cancellation)
        {
            var users = await _userService.GetAll(offset, count, cancellation);

            return Ok(users);
        }

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDetails>> GetById(Guid id, CancellationToken cancellation)
        {
            var user = await _userService.GetById(id, cancellation);

            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            return Ok(user);
        }

        /// <summary>
        /// Получить текущего пользователя.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<UserDetails>> GetCurrent(CancellationToken cancellation)
        {
            var user = await _userService.GetCurrent(cancellation);

            return Ok(user);
        }

        /// <summary>
        /// Изменить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<UserDetails>> Update(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var updatedUser = await _userService.UpdateAsync(id, updateRequest, cancellation);

            return Ok(updatedUser);
        }

        ///// <summary>
        ///// Изменить пользователя по идентификатору.
        ///// </summary>
        ///// <param name="newEmail">Новая электронная почта пользователя.</param>
        ///// <param name="token">Сгенерированный токен для изменения почты пользователя.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        //[HttpGet("change-email")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize]
        //public async Task<IActionResult> ChangeEmail([FromQuery] UserEmailDto newEmail, string token, CancellationToken cancellation)
        //{
        //    await _userService.ChangeEmailAsync(newEmail, token, cancellation);

        //    return Ok();
        //}

        ///// <summary>
        ///// Изменить пользователя по идентификатору.
        ///// </summary>
        ///// <param name="request">Элемент <see cref="UserChangeEmailDto"/>.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        //[HttpPost("change-email-request")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize]
        //public async Task<IActionResult> ChangeEmailRequest([FromBody] UserChangeEmailDto request, CancellationToken cancellation)
        //{
        //    var changeLink = Url.Action(nameof(ChangeEmail), "User", new { newEmail = request.newEmail, token = "tokenValue" }, Request.Scheme);

        //    await _userService.ChangeEmailRequestAsync(request, changeLink, cancellation);

        //    return Ok();
        //}

        ///// <summary>
        ///// Изменить пользователя по идентификатору.
        ///// </summary>
        ///// <param name="userChangePasswordDto">Элемент <see cref="UserChangePasswordDto"/>.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        //[HttpPut("change-password")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize]
        //public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto userChangePasswordDto, CancellationToken cancellation)
        //{
        //    await _userService.ChangePasswordAsync(userChangePasswordDto, cancellation);

        //    return Ok();
        //}

        ///// <summary>
        ///// Изменить пользователя по идентификатору.
        ///// </summary>
        ///// <param name="email">Текущая электронная почта пользователя.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        //[HttpGet("reset-password-request")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPasswordRequest([FromQuery] UserEmailDto email, CancellationToken cancellation)
        //{
        //    var resetLink = Url.Action(nameof(ResetPasswordConfirm), "User", new { email = email.Value, token = "tokenValue" }, Request.Scheme);

        //    await _userService.ResetPasswordRequestAsync(email, resetLink, cancellation);

        //    return Ok();
        //}

        ///// <summary>
        ///// Изменить пользователя по идентификатору.
        ///// </summary>
        ///// <param name="email">Электронная почта пользователя.</param>
        ///// <param name="token">Сгенерированный токен для сброса пароля.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        //[HttpGet("reset-password-confirm")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPasswordConfirm(string email, string token, CancellationToken cancellation)
        //{
        //    return Ok(new { email, token });
        //}

        ///// <summary>
        ///// Изменить пользователя по идентификатору.
        ///// </summary>
        ///// <param name="request">Элемент <see cref="UserResetPasswordDto"/>.</param>
        ///// <param name="token">Идентификатор пользователя.</param>
        ///// <param name="cancellation">Токен отмены.</param>
        //[HttpPut("reset-password")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword(UserResetPasswordDto request, string token, CancellationToken cancellation)
        //{
        //    await _userService.ResetPasswordAsync(request, token, cancellation);

        //    return Ok();
        //}



        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellation)
        {
            await _userService.DeleteAsync(id, cancellation);

            return Ok();
        }



        /// <summary>
        /// Зарегистрировать пользователя.
        /// </summary>
        /// <param name="userRegisterDto">Элемент <see cref="UserRegisterDto"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового пользователя.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Register(UserRegisterRequest registerRequest, CancellationToken cancellation)
        {
            var userId = await _userService.Register(registerRequest, cancellation);

            return Ok(userId);
        }

        /// <summary>
        /// Залогинить пользователя.
        /// </summary>
        /// <param name="userLoginDto">Элемент <see cref="UserLoginDto"/>.</param>
        /// <param name="cancellation"></param>
        /// <returns>Токен аутентификации.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login(UserLoginRequest loginRequest, CancellationToken cancellation)
        {
            
            var token = await _userService.Login(loginRequest, cancellation);

            return Ok(token);
        }



    }
}
