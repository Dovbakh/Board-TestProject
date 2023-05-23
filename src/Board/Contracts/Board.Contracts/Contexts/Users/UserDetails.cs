using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель с детальной информацией о пользователе.
    /// </summary>
    public class UserDetails
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Почта пользователя.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Рейтинг пользователя.
        /// </summary>
        public float Rating { get; set; }

        // <summary>
        /// Имя пользователя.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Дата создания пользователя.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Фото пользователя.
        /// </summary>
        public Guid? PhotoId { get; set; }

    }
}
