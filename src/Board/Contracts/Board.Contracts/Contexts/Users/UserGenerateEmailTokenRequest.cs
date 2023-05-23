using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель генерации подтверждения почты.
    /// </summary>
    public class UserGenerateEmailConfirmationTokenRequest
    {
        /// <summary>
        /// Почта.
        /// </summary>
        public string Email { get; set; }

    }
}
