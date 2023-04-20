using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Clients.Users
{
    public class UserDetailsClientResponse
    {
        public Guid? Id { get; set; }

        public string? Email { get; set; }

        public string? UserName { get; set; }

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
    }
}
