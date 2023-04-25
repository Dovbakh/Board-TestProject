using Identity.Contracts.Contexts.Users;
using Identity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.AppData.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Получить всех пользователей с пагинацией.
        /// </summary>
        /// <param name="take">Количество получаемых пользователей.</param>
        /// <param name="skip">Количество пропускаемых пользователей.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="UserDto"/>.</returns>
        Task<IReadOnlyCollection<UserSummary>> GetAll(int offset, int count, CancellationToken cancellationToken);

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        Task<UserDetails> GetById(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Получить пользователя по почте.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Элемент <see cref="UserDto"/>.</returns>
        Task<UserDetails> GetByEmail(string email, CancellationToken cancellationToken);

        /// <summary>
        /// Добавить пользователя с хэшем пароля.
        /// </summary>
        /// <param name="userDto">Элемент <see cref="UserDto"/>.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Идентификатор нового пользователя.</returns>
        Task<Guid> AddAsync(UserRegisterRequest registerRequest, CancellationToken cancellationToken);


        /// <summary>
        /// Изменить пользователя.
        /// </summary>
        /// <param name="request">Элемент <see cref="UserUpdateRequestDto"/>.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken);


        /// <summary>
        /// Удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Проверить пароль пользователя.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken);

        /// <summary>
        /// Изменить пароль у пользователя.
        /// </summary>
        /// <param name="email">Электронная почта текущего пользователя.</param>
        /// <param name="currentPassword">Текущий пароль текущего пользователя.</param>
        /// <param name="newPassword">Новый пароль текущего пользователя.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns></returns>
        Task ChangePasswordAsync(string email, string currentPassword, string newPassword, CancellationToken cancellationToken);

        /// <summary>
        /// Изменить электронную почту у пользователя.
        /// </summary>
        /// <param name="currentEmail">Текущая электронная почта пользователя.</param>
        /// <param name="newEmail">Новая электронная почта пользователя.</param>
        /// <param name="token">Сгенерированный токен смены почты.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns></returns>
        Task ChangeEmailAsync(string currentEmail, string newEmail, string token, CancellationToken cancellationToken);

        /// <summary>
        /// Получение токена для изменения почты пользователя.
        /// </summary>
        /// <param name="currentEmail">Текущая почта пользователя.</param>
        /// <param name="newEmail">Новая почта пользователя.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Токен для изменения почты пользователя.</returns>
        Task<string> ChangeEmailRequestAsync(string currentEmail, string newEmail, CancellationToken cancellationToken);

        /// <summary>
        /// Получение токена для сброса пароля пользователя.
        /// </summary>
        /// <param name="email">Почта пользователя.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Токен для сброса пароля пользователя.</returns>
        Task<string> ResetPasswordRequestAsync(string email, CancellationToken cancellationToken);

        /// <summary>
        /// Сброс пароля пользователя.
        /// </summary>
        /// <param name="email">Почта пользователя.</param>
        /// <param name="token">Сгенерированный токен сброса пароля.</param>
        /// <param name="newPassword">Новый пароль.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns></returns>
        Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);

        Task<bool> IsInRoleRole(Guid userId, string role, CancellationToken cancellationToken);

    }

}

