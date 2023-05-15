using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    public class CookieOptions
    {
        public string AnonymousUserKey { get; set; } = "AnonymousUserKey_";
        public string AnonymousFavoriteKey { get; set; } = "AnonymousFavoriteKey_";
    }
}
