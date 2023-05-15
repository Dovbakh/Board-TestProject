using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    public class AdvertFavoriteAddLockOptions
    {
        public string AdvertFavoriteAddKey { get; set; } = "AdvertFavoriteAddKey_";
        public TimeSpan Expire { get; set; }
        public TimeSpan Wait { get; set; }
        public TimeSpan Retry { get; set; }
    }
}
