using Identity.Contracts.Contexts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.AppData.Services
{
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

        Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellation);

        /// <summary>
        /// Получение токена для изменения почты пользователя и его отправка на новую почту.
        /// </summary>
        /// <param name="changeEmailRequest">Элемент <see cref="UserChangeEmailRequest"/>.</param>
        /// /// <param name="changeLink">Шаблон ссылки на изменение почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Токен для изменения почты пользователя.</returns>
        Task<EmailChangeToken> GenerateEmailTokenAsync(UserGenerateEmailTokenRequest changeEmailRequest, CancellationToken cancellation);

        Task<EmailConfirmationToken> GenerateEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellation);

        /// <summary>
        /// Изменить электронную почту у пользователя.
        /// </summary>
        /// <param name="newEmail">Элемент <see cref="UserEmailDto"/>.</param>
        /// <param name="token">Сгенерированный токен смены почты.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        Task ChangeEmailAsync(UserChangeEmailRequest request, CancellationToken cancellation);

        Task ConfirmEmailAsync(UserConfirmEmailRequest request, CancellationToken cancellation);

    }
}
