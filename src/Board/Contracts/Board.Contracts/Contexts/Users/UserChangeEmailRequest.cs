using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель изменения почты.
    /// </summary>
    public class UserChangeEmailRequest
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
        /// Токен изменения почты.
        /// </summary>
        public string Token { get; set; }
    }
}
