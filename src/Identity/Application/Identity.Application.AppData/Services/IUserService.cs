using Identity.Contracts.Contexts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.AppData.Services
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
        /// Изменить пользователя.
        /// </summary>
        /// <param name="updateRequest">Элемент <see cref="UserUpdateRequest"/>.</param>
        /// <param name="cancellation"></param>
        Task<UserDetails> UpdateAsync(Guid userId, UserUpdateRequest updateRequest, CancellationToken cancellation);

        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены.</param>
        Task DeleteAsync(Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Проверка пользователя на наличие роли.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellation);

        /// <summary>
        /// Получение токена для изменения почты пользователя и его отправка на новую почту.
        /// </summary>
        /// <param name="changeEmailRequest">Элемент <see cref="UserChangeEmailRequest"/>.</param>
        /// /// <param name="changeLink">Шаблон ссылки на изменение почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Токен для изменения почты пользователя.</returns>
        Task<EmailChangeToken> GenerateEmailTokenAsync(UserGenerateEmailTokenRequest changeEmailRequest, CancellationToken cancellation);

        /// <summary>
        /// Получение токена для подтверждения почты пользователя и его отправка на новую почту.
        /// </summary>
        /// <param name="changeEmailRequest">Элемент <see cref="UserChangeEmailRequest"/>.</param>
        /// /// <param name="changeLink">Шаблон ссылки на изменение почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Токен для изменения почты пользователя.</returns>
        Task<EmailConfirmationToken> GenerateEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellation);

        /// <summary>
        /// Изменить электронную почту у пользователя.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        Task ChangeEmailAsync(UserChangeEmailRequest request, CancellationToken cancellation);

        /// <summary>
        /// Подтвердить электронную почту у пользователя.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        Task ConfirmEmailAsync(UserConfirmEmailRequest request, CancellationToken cancellation);

    }
}
