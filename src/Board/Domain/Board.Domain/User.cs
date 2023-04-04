using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class User : IdentityUser<Guid>
    {

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
        /// Идентификатор роли пользователя.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Коллекция обьявлений пользователя.
        /// </summary>
        public ICollection<Advert> Posts { get; set; }

        /// <summary>
        /// Коллекция отзывов, написанных пользователем.
        /// </summary>
        public ICollection<Comment> CommentsBy { get; set; }

        /// <summary>
        /// Коллекция отзывов, написанных о пользователе.
        /// </summary>
        public ICollection<Comment> CommentsFor { get; set; }

        public bool isActive { get; set; }
    }
}
