using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель регистрации пользователя.
    /// </summary>
    public class UserRegisterRequest
    {
        /// <summary>
        /// Почта пользователя.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля пользователя.
        /// </summary>
        public string? PasswordConfirm { get; set; }
    }
}
