using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.Users;
using Board.Contracts.Contexts.Users.Enums;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с пользователями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IReadOnlyCollection<UserSummary>>> GetAll(int offset, int count, CancellationToken cancellation)
        {
            var users = await _userService.GetAllAsync(offset, count, cancellation);

            return Ok(users);
        }

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        [HttpGet("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDetails>> GetById(Guid id, CancellationToken cancellation)
        {
            var user = await _userService.GetByIdAsync(id, cancellation);

            if(user == null)
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
            var user = await _userService.GetCurrentAsync(cancellation);

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

        /// <summary>
        /// Изменить пользователя по идентификатору.
        /// </summary>
        /// <param name="newEmail">Новая электронная почта пользователя.</param>
        /// <param name="token">Сгенерированный токен для изменения почты пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("change-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> ChangeEmail([FromQuery] string newEmail, string token, CancellationToken cancellation)
        {
            await _userService.ChangeEmailAsync(newEmail, token, cancellation);

            return Ok();
        }

        /// <summary>
        /// Изменить пользователя по идентификатору.
        /// </summary>
        /// <param name="request">Элемент <see cref="UserChangeEmailDto"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("send-email-change-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> SendEmailTokenAsync([FromBody] UserGenerateEmailTokenRequest request, CancellationToken cancellation)
        {
            var changeLink = Url.Action(nameof(ChangeEmail), "User", new { newEmail = request.NewEmail, token = "tokenValue" }, Request.Scheme);

            await _userService.SendEmailTokenAsync(request, changeLink, cancellation);

            return Ok();
        }

        /// <summary>
        /// Изменить пользователя по идентификатору.
        /// </summary>
        /// <param name="request">Элемент <see cref="UserChangeEmailDto"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("send-email-confirmation-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> SendEmailConfirmationTokenAsync([FromBody] UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellation)
        {
            var changeLink = Url.Action(nameof(ConfirmEmail), "User", new { email = request.Email, token = "tokenValue" }, Request.Scheme);

            await _userService.SendEmailConfirmationTokenAsync(request, changeLink, cancellation);

            return Ok();
        }


        /// <summary>
        /// Изменить пользователя по идентификатору.
        /// </summary>
        /// <param name="newEmail">Новая электронная почта пользователя.</param>
        /// <param name="token">Сгенерированный токен для изменения почты пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, string token, CancellationToken cancellation)
        {
            await _userService.ConfirmEmailAsync(email, token, cancellation);

            return Ok();
        }
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
            var userId = await _userService.RegisterAsync(registerRequest, cancellation);

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
            var token = await _userService.LoginAsync(loginRequest, cancellation);

            return Ok(token);
        }



    }

}
