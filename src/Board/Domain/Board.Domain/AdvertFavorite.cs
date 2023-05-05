using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class AdvertFavorite
    {
        public Guid Id { get; set; }
        public Guid AdvertId { get; set; }
        public Guid UserId { get; set; }

        public Advert Advert { get; set; }
    }
}
