using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Options
{
    public class UserOptions
    {
        public int ListDefaultCount { get; set; } = 10;
        public string UserIdCacheKey { get; set; } = "UserId_";
    }
}
