using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Contexts.Users;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Users.Services
{
    /// <summary>
    /// Сервис для работы с пользователями.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <param name="registerRequest">Элемент <see cref="UserRegisterRequest"/>.</param>
        /// <param name="cancellation"></param>
        /// <returns>Идентификатор нового пользователя.</returns>
        Task<Guid> RegisterAsync(UserRegisterRequest registerRequest, CancellationToken cancellation);

        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        /// <param name="loginRequest">Элемент <see cref="UserLoginRequest"/>.</param>
        /// <param name="cancellation"></param>
        /// <returns>Токен.</returns>
        Task<TokenResponse> LoginAsync(UserLoginRequest loginRequest, CancellationToken cancellation);

        Task<TokenResponse> LoginAsync(UserLoginRefreshRequest loginRefreshRequest, CancellationToken cancellation);

        Task LogoutAsync(CancellationToken cancellation);

        /// <summary>
        /// Получить всех пользователей с пагинацией.
        /// </summary>
        /// <param name="take">Количество получаемых пользователей.</param>
        /// <param name="skip">Количество пропускаемых пользователей.</param>
        /// <param name="cancellation"></param>
        /// <returns>Коллекция элементов <see cref="UserSummary"/>.</returns>
        Task<IReadOnlyCollection<UserSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation);

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDetails"/>.</returns>
        Task<UserDetails> GetByIdAsync(Guid id, CancellationToken cancellation);

        Task<IReadOnlyCollection<AdvertSummary>> GetAdvertsByUserIdAsync(Guid id, int? offset, int? limit, CancellationToken cancellation);

        Task<IReadOnlyCollection<CommentDetails>> GetCommentsByReceiverUserIdAsync(Guid id, int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Получить текущего пользователя.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns>Элемент <see cref="UserDetails"/></returns>
        Task<UserDetails> GetCurrentAsync(CancellationToken cancellation);

        Guid? GetCurrentId(CancellationToken cancellation);

        Task<bool> IsLoginedAsync(CancellationToken cancellation);

        bool HasPermission(Guid userId, CancellationToken cancellation);

        bool IsCurrentUser(Guid userId, CancellationToken cancellation);

        bool IsAdmin(Guid userId, CancellationToken cancellation);

        Guid GetAnonymousId(CancellationToken cancellation);

        /// <summary>
        /// Изменить пользователя.
        /// </summary>
        /// <param name="updateRequest">Элемент <see cref="UserUpdateRequest"/>.</param>
        /// <param name="cancellation"></param>
        Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid id, CancellationToken cancellation);


        /// <summary>
        /// Получение токена для изменения почты пользователя и его отправка на новую почту.
        /// </summary>
        /// <param name="changeEmailRequest">Элемент <see cref="UserChangeEmailRequest"/>.</param>
        /// /// <param name="changeLink">Шаблон ссылки на изменение почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Токен для изменения почты пользователя.</returns>
        Task SendEmailTokenAsync(UserGenerateEmailTokenRequest changeEmailRequest, string changeLink, CancellationToken cancellation);

        Task SendEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, string confirmLink, CancellationToken cancellation);

        /// <summary>
        /// Изменить электронную почту у пользователя.
        /// </summary>
        /// <param name="newEmail">Элемент <see cref="UserEmailDto"/>.</param>
        /// <param name="token">Сгенерированный токен смены почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        Task ChangeEmailAsync(string newEmail, string token, CancellationToken cancellation);

        public Task ConfirmEmailAsync(string email, string token, CancellationToken cancellation);


    }

}
