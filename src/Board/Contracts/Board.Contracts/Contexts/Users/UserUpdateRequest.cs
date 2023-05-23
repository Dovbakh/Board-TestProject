using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель изменения пользователя.
    /// </summary>
    public class UserUpdateRequest
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Фото пользователя.
        /// </summary>
        public Guid? PhotoId { get; set; }
    }
}
