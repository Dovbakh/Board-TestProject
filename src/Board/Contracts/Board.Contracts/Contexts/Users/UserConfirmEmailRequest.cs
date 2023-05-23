using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель подтверждения почты.
    /// </summary>
    public class UserConfirmEmailRequest
    {
        /// <summary>
        /// Почта.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Токен подтверждения почты.
        /// </summary>
        public string Token { get; set; }
    }
}
