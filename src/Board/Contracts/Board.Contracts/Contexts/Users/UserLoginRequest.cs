using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель логина пользователя.
    /// </summary>
    public class UserLoginRequest
    {
        /// <summary>
        /// Логин пользователя.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }
    }
}
