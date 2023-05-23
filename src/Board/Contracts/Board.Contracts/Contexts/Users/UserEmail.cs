using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель с почтой пользователя.
    /// </summary>
    public class UserEmail
    {
        /// <summary>
        /// Почта.
        /// </summary>
        public string Value { get; set; }
    }
}
