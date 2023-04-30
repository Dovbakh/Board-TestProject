using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Clients.Users
{
    public class UserGenerateEmailTokenClientRequest
    {
        public string CurrentEmail { get; set; }
        /// <summary>
        /// Новая почта пользователя.
        /// </summary>
        public string NewEmail { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }
    }
}
