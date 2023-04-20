using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Clients.Users
{
    public class UserRegisterClientRequest
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
