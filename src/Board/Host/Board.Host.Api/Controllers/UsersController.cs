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
    /// Контроллер для работы с пользователями.
    /// </summary>
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Конструктор контроллера для работы с пользователями.
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями.</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Получить список всех пользователей с пагинацией. [admin only]
        /// </summary>
        /// <param name="limit">Количество получаемых пользователей.</param>
        /// <param name="offset">Количество пропускаемых пользователей.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список пользователей.</returns>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IReadOnlyCollection<UserSummary>>> GetAll(int? offset, int? limit, CancellationToken cancellation)
        {
            var users = await _userService.GetAllAsync(offset, limit, cancellation);

            return Ok(users);
        }

        /// <summary>
        /// Получить пользователя по идентификатору. [anonymous]
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Пользователь с детальной информацией.</returns>
        [HttpGet("{userId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDetails>> GetById(Guid userId, CancellationToken cancellation)
        {
            var user = await _userService.GetByIdAsync(userId, cancellation);

            return Ok(user);
        }

        /// <summary>
        /// Получить обьявления пользователя по идентификатору с пагинацией. [anonymous]
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений.</returns>
        [HttpGet("{userId:Guid}/adverts")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAdvertsByUserId(Guid userId, int? offset, int? limit, CancellationToken cancellation)
        {
            var adverts = await _userService.GetAdvertsByUserIdAsync(userId, offset, limit, cancellation);

            return Ok(adverts);
        }

        /// <summary>
        /// Получить отзывы, оставленные пользователю по идентификатору с пагинацией. [anonymous]
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список отзывов.</returns>
        [HttpGet("{userId:Guid}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CommentDetails>>> GetCommentsByReceiverUserId(Guid userId, int? offset, int? limit, CancellationToken cancellation)
        {
            var adverts = await _userService.GetCommentsByReceiverUserIdAsync(userId, offset, limit, cancellation);

            return Ok(adverts);
        }

        /// <summary>
        /// Получить текущего пользователя. [authorize]
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Пользователь с детальной информацией.</returns>
        [HttpGet("current")]
        public async Task<ActionResult<UserDetails>> GetCurrent(CancellationToken cancellation)
        {
            var user = await _userService.GetCurrentAsync(cancellation);

            return Ok(user);
        }

        /// <summary>
        /// Изменить текущего пользователя. [authorize]
        /// </summary>
        /// <param name="updateRequest">Модель изменения пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Измененный пользователь с детальной информацией.</returns>
        [HttpPut]
        public async Task<ActionResult<UserDetails>> UpdateCurrent(UserUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var updatedUser = await _userService.UpdateCurrentAsync(updateRequest, cancellation);

            return Ok(updatedUser);
        }

        /// <summary>
        /// Изменить почту. [authorize]
        /// </summary>
        /// <param name="newEmail">Новая электронная почта.</param>
        /// <param name="token">Токен для изменения почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpGet("change-email")]
        public async Task<IActionResult> ChangeEmail([FromQuery] string newEmail, string token, CancellationToken cancellation)
        {
            await _userService.ChangeEmailAsync(newEmail, token, cancellation);

            return Ok();
        }

        /// <summary>
        /// Отправить токен для изменения почты. [authorize]
        /// </summary>
        /// <param name="request">Модель генерации и отправки токена для изменения почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("send-email-change-token")]
        public async Task<IActionResult> SendEmailTokenAsync([FromBody] UserGenerateEmailTokenRequest request, CancellationToken cancellation)
        {
            var changeLink = Url.Action(nameof(ChangeEmail), "User", new { newEmail = request.NewEmail, token = "tokenValue" }, Request.Scheme);
            await _userService.SendEmailTokenAsync(request, changeLink, cancellation);

            return Ok();
        }

        /// <summary>
        /// Отправить токен для подтверждения почты. [authorize]
        /// </summary>
        /// <param name="request">Модель генерации и отправки токена для подтверждения почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("send-email-confirmation-token")]
        public async Task<IActionResult> SendEmailConfirmationTokenAsync([FromBody] UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellation)
        {
            var confirmLink = Url.Action(nameof(ConfirmEmail), this.ControllerContext.RouteData.Values["controller"].ToString(), new { email = request.Email, token = "tokenValue" }, Request.Scheme);
            await _userService.SendEmailConfirmationTokenAsync(request, confirmLink, cancellation);

            return Ok();
        }


        /// <summary>
        /// Подтвердить почту. [anonymous]
        /// </summary>
        /// <param name="email">Почта пользователя.</param>
        /// <param name="token">Токен для подтверждения почты пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, string token, CancellationToken cancellation)
        {
            await _userService.ConfirmEmailAsync(email, token, cancellation);

            return Ok();
        }

        /// <summary>
        /// Удалить пользователя по идентификатору. [authorize]
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{userId:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellation)
        {
            await _userService.DeleteAsync(userId, cancellation);

            return NoContent();
        }



        /// <summary>
        /// Зарегистрировать пользователя. [anonymous]
        /// </summary>
        /// <param name="registerRequest">Модель регистрации пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового пользователя.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Register(UserRegisterRequest registerRequest, CancellationToken cancellation)
        {
            var userId = await _userService.RegisterAsync(registerRequest, cancellation);

            return CreatedAtAction(nameof(Register), userId);
        }

        /// <summary>
        /// Залогинить пользователя. [anonymous]
        /// </summary>
        /// <param name="loginRequest">Модель логина с почтой и паролем.</param>
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
        /// Залогинить пользователя c рефреш токеном.
        /// </summary>
        /// <param name="loginRefreshRequest">Модель логина с рефреш токеном.</param>
        /// <param name="cancellation"></param>
        /// <returns>Токен аутентификации.</returns>
        [HttpPost("login-refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponse>> Login([FromForm] UserLoginRefreshRequest loginRefreshRequest, CancellationToken cancellation)
        {
            var token = await _userService.LoginAsync(loginRefreshRequest, cancellation);

            return Ok(token.Json);
        }

        /// <summary>
        /// Разлогинить текущего пользователя. [authorize]
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns>Токен аутентификации.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellation)
        {
            await _userService.LogoutAsync(cancellation);

            return Ok();
        }

    }

}
