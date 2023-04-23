using Board.Contracts.Contexts.Users;
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
        /// <param name="cancellationToken"></param>
        /// <returns>Идентификатор нового пользователя.</returns>
        Task<Guid> Register(UserRegisterRequest registerRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        /// <param name="loginRequest">Элемент <see cref="UserLoginRequest"/>.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Токен.</returns>
        Task<string> Login(UserLoginRequest loginRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Получить всех пользователей с пагинацией.
        /// </summary>
        /// <param name="take">Количество получаемых пользователей.</param>
        /// <param name="skip">Количество пропускаемых пользователей.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Коллекция элементов <see cref="UserSummary"/>.</returns>
        Task<IReadOnlyCollection<UserSummary>> GetAll(int? offset, int? count, CancellationToken cancellationToken);

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Элемент <see cref="UserDetails"/>.</returns>
        Task<UserDetails> GetById(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Получить текущего пользователя.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Элемент <see cref="UserDetails"/></returns>
        Task<UserDetails> GetCurrent(CancellationToken cancellationToken);

        Task<Guid> GetCurrentId(CancellationToken cancellationToken);

        /// <summary>
        /// Изменить пользователя.
        /// </summary>
        /// <param name="updateRequest">Элемент <see cref="UserUpdateRequest"/>.</param>
        /// <param name="cancellationToken"></param>
        Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        ///// <summary>
        ///// Изменить пароль у пользователя.
        ///// </summary>
        ///// <param name="changePasswordRequest">Элемент <see cref="UserChangePasswordRequest"/>.</param>
        ///// <param name="cancellationToken">Токен отмены.</param>
        ///// <returns></returns>
        //Task ChangePasswordAsync(UserChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken);

        ///// <summary>
        ///// Получение токена для изменения почты пользователя и его отправка на новую почту.
        ///// </summary>
        ///// <param name="changeEmailRequest">Элемент <see cref="UserChangeEmailRequest"/>.</param>
        ///// /// <param name="changeLink">Шаблон ссылки на изменение почты.</param>
        ///// <param name="cancellationToken">Токен отмены.</param>
        ///// <returns>Токен для изменения почты пользователя.</returns>
        //Task ChangeEmailRequestAsync(UserChangeEmailRequest changeEmailRequest, string changeLink, CancellationToken cancellationToken);

        ///// <summary>
        ///// Изменить электронную почту у пользователя.
        ///// </summary>
        ///// <param name="newEmail">Элемент <see cref="UserEmailDto"/>.</param>
        ///// <param name="token">Сгенерированный токен смены почты.</param>
        ///// <param name="cancellationToken">Токен отмены.</param>
        ///// <returns></returns>
        //Task ChangeEmailAsync(UserEmailDto newEmail, string token, CancellationToken cancellationToken);

        ///// <summary>
        ///// Получение токена для сброса пароля пользователя и его отправка на почту.
        ///// </summary>
        ///// <param name="email">Почта пользователя.</param>
        ///// <param name="resetLink">Шаблон для ссылки на сброс пароля.</param>
        ///// <param name="cancellationToken">Токен отмены.</param>
        ///// <returns></returns>
        //Task ResetPasswordRequestAsync(UserEmailDto email, string resetLink, CancellationToken cancellationToken);

        ///// <summary>
        ///// Сброс пароля пользователя.
        ///// </summary>
        ///// <param name="request">Элемент <see cref="UserResetPasswordDto"/>.</param>
        ///// <param name="token">Сгенерированный токен на сброс пароля.</param>
        ///// <param name="cancellationToken">Токен отмены.</param>
        ///// <returns></returns>
        //Task ResetPasswordAsync(UserResetPasswordDto request, string token, CancellationToken cancellationToken);

    }

}
