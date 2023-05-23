using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель генерации изменения почты.
    /// </summary>
    public class UserGenerateEmailTokenRequest
    {
        /// <summary>
        /// Текущая почта.
        /// </summary>
        public string CurrentEmail { get; set; }

        /// <summary>
        /// Новая почта.
        /// </summary>
        public string NewEmail { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }
    }
}
