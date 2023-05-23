using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Clients.Users
{
    public class UserChangeEmailClientRequest
    {
        public string CurrentEmail { get; set; }
        public string NewEmail { get; set; }
        public string Token { get; set; }
    }
}
