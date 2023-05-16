﻿using Identity.Application.AppData.Services;
using Identity.Contracts.Contexts.Users;
using Identity.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Identity.Host.Server.Controllers
{
    /// <summary>
    /// Работа с пользователями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    [Authorize(AuthenticationSchemes = "Bearer")]
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
        [HttpGet("{id}")]
        //[AllowAnonymous]
        public async Task<ActionResult<UserDetails>> GetById(Guid id, CancellationToken cancellation)
        {
            var user = await _userService.GetByIdAsync(id, cancellation);

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
        public async Task<IActionResult> ChangeEmail([FromBody]UserChangeEmailRequest request, CancellationToken cancellation)
        {
            await _userService.ChangeEmailAsync(request, cancellation);

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
        public async Task<IActionResult> ConfirmEmail([FromBody] UserConfirmEmailRequest request, CancellationToken cancellation)
        {
            await _userService.ConfirmEmailAsync(request, cancellation);

            return Ok();
        }

        /// <summary>
        /// Изменить пользователя по идентификатору.
        /// </summary>
        /// <param name="request">Элемент <see cref="UserChangeEmailDto"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("generate-email-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailChangeToken>> GenerateEmailTokenAsync([FromBody] UserGenerateEmailTokenRequest request, CancellationToken cancellation)
        {
            var token = await _userService.GenerateEmailTokenAsync(request, cancellation);

            return Ok(token);
        }

        /// <summary>
        /// Изменить пользователя по идентификатору.
        /// </summary>
        /// <param name="request">Элемент <see cref="UserChangeEmailDto"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost("generate-email-confirmation-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailConfirmationToken>> GenerateEmailConfirmationTokenAsync([FromBody] UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellation)
        {
            var token = await _userService.GenerateEmailConfirmationTokenAsync(request, cancellation);

            return Ok(token);
        }

        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        public async Task<ActionResult<Guid>> Register(UserRegisterRequest registerRequest, CancellationToken cancellation)
        {
            var userId = await _userService.RegisterAsync(registerRequest, cancellation);

            return Ok(userId);
        }
    }
}
