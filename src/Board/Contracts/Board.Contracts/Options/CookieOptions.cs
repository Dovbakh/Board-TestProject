using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции для работы с куками.
    /// </summary>
    public class CookieOptions
    {
        /// <summary>
        /// Ключ куков, в которых лежит идентификатор неавторизованного пользователя.
        /// </summary>
        public string AnonymousUserKey { get; set; } = "AnonymousUserKey_";

        /// <summary>
        /// Ключ куков, в которых лежит избранное неавторизованного пользователя.
        /// </summary>
        public string AnonymousFavoriteKey { get; set; } = "AnonymousFavoriteKey_";
    }
}
