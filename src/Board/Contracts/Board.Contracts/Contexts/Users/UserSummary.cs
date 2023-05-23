using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель с краткой информацией о пользователе.
    /// </summary>
    public class UserSummary
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Почта пользователя.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string? Name { get; set; }

    }
}
