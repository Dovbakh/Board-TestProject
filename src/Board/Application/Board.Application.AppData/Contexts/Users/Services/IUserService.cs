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
        /// <param name="registerRequest">Модель регистрации пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового пользователя.</returns>
        Task<Guid> RegisterAsync(UserRegisterRequest registerRequest, CancellationToken cancellation);

        /// <summary>
        /// Авторизация пользователя с логином и паролем.
        /// </summary>
        /// <param name="loginRequest">Модель авторизации пользователя с логином и паролем.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Модель, содержащая access_token, refresh_token и expires.</returns>
        Task<TokenResponse> LoginAsync(UserLoginRequest loginRequest, CancellationToken cancellation);

        /// <summary>
        /// Авторизация пользователя с рефреш токеном.
        /// </summary>
        /// <param name="loginRefreshRequest">Модель авторизации пользователя с рефреш токеном.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Модель, содержащая access_token, refresh_token и expires.</returns>
        Task<TokenResponse> LoginAsync(UserLoginRefreshRequest loginRefreshRequest, CancellationToken cancellation);

        /// <summary>
        /// Удаление авторизационных куков из сессии.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        Task LogoutAsync(CancellationToken cancellation);

        /// <summary>
        /// Получить всех пользователей с пагинацией.
        /// </summary>
        /// <param name="limit">Количество получаемых пользователей.</param>
        /// <param name="offset">Количество пропускаемых пользователей.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список пользователей с краткой информацией.</returns>
        Task<IReadOnlyCollection<UserSummary>> GetAllAsync(int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Детальная информация о пользователе.</returns>
        Task<UserDetails> GetByIdAsync(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Получить все обьявления пользователя с пагинацией.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений с краткой информацией.</returns>
        Task<IReadOnlyCollection<AdvertSummary>> GetAdvertsByUserIdAsync(Guid userId, int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Получить все отзывы, оставленные пользователю с пагинацией.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="limit">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список отзывов.</returns>
        Task<IReadOnlyCollection<CommentDetails>> GetCommentsByReceiverUserIdAsync(Guid userId, int? offset, int? limit, CancellationToken cancellation);

        /// <summary>
        /// Получить текущего пользователя.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Детальная информация о пользователе.</returns>
        Task<UserDetails> GetCurrentAsync(CancellationToken cancellation);

        /// <summary>
        /// Получить идентификатор текущего пользователя.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns>Идентификатор пользователя, если он авторизован. null - если не авторизован.</returns>
        Guid? GetCurrentId(CancellationToken cancellation);

        /// <summary>
        /// Проверка пользователя на логин и валидацию токена.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>true - если логин подтверждение, false - не авторизован.</returns>
        Task<bool> IsLoginedAsync(CancellationToken cancellation);

        /// <summary>
        /// Проверка текущего пользователя на наличие доступа к ресурсу указанного пользователя с идентификатором.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя-владельца ресурса.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        bool HasPermission(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Проверка принадлежит ли указанный идентификатор текущему пользователю.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        bool IsCurrentUser(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Проверка является ли указанный пользователь админом.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        bool IsAdmin(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Получить идентификатор неавторизованного пользователя из куков. Если не найден, то сгенерировать новый, положить в куки и вернуть. 
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор неавторизованного пользователя.</returns>
        Guid GetAnonymousId(CancellationToken cancellation);

        /// <summary>
        /// Изменить текущего пользователя.
        /// </summary>
        /// <param name="updateRequest">Модель изменения пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Измененная детальная информация о пользователе.</returns>
        Task<UserDetails> UpdateCurrentAsync(UserUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Генерация токена для изменения почты пользователя и отправление на почту.
        /// </summary>
        /// <param name="request">Модель генерации токена для изменения почты.</param>
        /// /// <param name="changeLink">Ссылка на метод контроллера для изменения почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task SendEmailTokenAsync(UserGenerateEmailTokenRequest request, string changeLink, CancellationToken cancellation);

        /// <summary>
        /// Генерация токена для подтверждения почты пользователя и отправление на почту.
        /// </summary>
        /// <param name="request">Модель генерации токена для подтверждения почты.</param>
        /// <param name="confirmLink">Ссылка на метод контроллера для подтверждения почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task SendEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, string confirmLink, CancellationToken cancellation);

        /// <summary>
        /// Изменить почту текущего пользователя.
        /// </summary>
        /// <param name="newEmail">Новая почта.</param>
        /// <param name="token">Токен смены почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task ChangeEmailAsync(string newEmail, string token, CancellationToken cancellation);

        /// <summary>
        /// Подтвердить указанную почту пользователя.
        /// </summary>
        /// <param name="email">Подтверждаемая почта.</param>
        /// <param name="token">Токен подтверждения почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task ConfirmEmailAsync(string email, string token, CancellationToken cancellation);


    }

}
