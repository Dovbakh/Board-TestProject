using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Clients.Users
{
    public class UserGenerateEmailConfirmationTokenClientRequest
    {
        public string Email { get; set; }
    }
}
