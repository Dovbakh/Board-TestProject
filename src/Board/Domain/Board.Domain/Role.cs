using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class Role : IdentityRole<Guid>
    {
        /// <summary>
        /// Коллекция пользователей с этой ролью.
        /// </summary>
        public IReadOnlyCollection<User> Users { get; set; }
    }
}
