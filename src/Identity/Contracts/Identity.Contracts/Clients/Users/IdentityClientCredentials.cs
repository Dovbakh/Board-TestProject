using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Clients.Users
{
    public class IdentityClientCredentials
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }
}
