using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class AdvertView
    {
        public Guid Id { get; set; }
        public Guid AdvertId { get; set; }
        public Guid VisitorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isRegistered { get; set; }
        public Advert Advert { get; set; }
    }
}
