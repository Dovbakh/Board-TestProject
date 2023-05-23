using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Contexts.Users
{
    public class UserGenerateEmailConfirmationTokenRequest
    {
        public string Email { get; set; }

    }
}
