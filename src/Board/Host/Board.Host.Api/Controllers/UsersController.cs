using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Contexts.Users;
using Board.Contracts.Contexts.Users.Enums;
using Board.Contracts.Conventions;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с пользователями.
    /// </summary>
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Работа с пользователями.
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями.</param>
        public UsersController(IUserService userService)
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
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IReadOnlyCollection<UserSummary>>> GetAll(int? offset, int? count, CancellationToken cancellation)
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

            return Ok(user);
        }

        /// <summary>
        /// Получить обьявления пользователя с идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        [HttpGet("{id:Guid}/adverts")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAdvertsByUserId(Guid id, int? offset, int? limit, CancellationToken cancellation)
        {
            var adverts = await _userService.GetAdvertsByUserIdAsync(id, offset, limit, cancellation);

            return Ok(adverts);
        }

        /// <summary>
        /// Получить обьявления пользователя с идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        [HttpGet("{id:Guid}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CommentDetails>>> GetCommentsByReceiverUserId(Guid id, int? offset, int? limit, CancellationToken cancellation)
        {
            var adverts = await _userService.GetCommentsByReceiverUserIdAsync(id, offset, limit, cancellation);

            return Ok(adverts);
        }

        /// <summary>
        /// Получить текущего пользователя.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        [HttpGet("current")]
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
        public async Task<IActionResult> SendEmailConfirmationTokenAsync([FromBody] UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellation)
        {
            var confirmLink = Url.Action(nameof(ConfirmEmail), "User", new { email = request.Email, token = "tokenValue" }, Request.Scheme);

            await _userService.SendEmailConfirmationTokenAsync(request, confirmLink, cancellation);

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
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, string token, CancellationToken cancellation)
        {
            await _userService.ConfirmEmailAsync(email, token, cancellation);

            return Ok();
        }

        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellation)
        {
            await _userService.DeleteAsync(id, cancellation);

            return NoContent();
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
            var confirmLink = Url.Action(nameof(ConfirmEmail), "User", new { email = registerRequest.Email, token = "tokenValue" }, Request.Scheme);

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
        public async Task<ActionResult<string>> Login([FromForm] UserLoginRequest loginRequest, CancellationToken cancellation)
        {
            var token = await _userService.LoginAsync(loginRequest, cancellation);

            return Ok(token.Json);
        }

        /// <summary>
        /// Залогинить пользователя.
        /// </summary>
        /// <param name="userLoginDto">Элемент <see cref="UserLoginDto"/>.</param>
        /// <param name="cancellation"></param>
        /// <returns>Токен аутентификации.</returns>
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(CancellationToken cancellation)
        {
            await _userService.LogoutAsync(cancellation);

            return Ok();
        }

        /// <summary>
        /// Залогинить пользователя.
        /// </summary>
        /// <param name="userLoginDto">Элемент <see cref="UserLoginDto"/>.</param>
        /// <param name="cancellation"></param>
        /// <returns>Токен аутентификации.</returns>
        [HttpPost("login-refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponse>> Login([FromForm] UserLoginRefreshRequest loginRefreshRequest, CancellationToken cancellation)
        {
            var token = await _userService.LoginAsync(loginRefreshRequest, cancellation);
            
            return Ok(token.Json);
        }
    }

}
