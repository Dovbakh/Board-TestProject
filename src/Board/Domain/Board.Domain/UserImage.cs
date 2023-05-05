using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class UserImage
    {
        public Guid Id { get; set; }
        public Guid ImageId { get; set; }
        public Guid UserId { get; set; }
    }
}
