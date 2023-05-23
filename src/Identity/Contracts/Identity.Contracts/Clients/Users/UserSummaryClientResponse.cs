using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Clients.Users
{
    public class UserSummaryClientResponse
    {
        public Guid? Id { get; set; }

        public string? Email { get; set; }

        public string? Name { get; set; }
    }
}
