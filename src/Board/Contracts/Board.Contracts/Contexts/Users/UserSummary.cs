using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    public class UserSummary
    {
        public Guid? Id { get; set; }

        public string? Email { get; set; }

        public string? Name { get; set; }

    }
}
