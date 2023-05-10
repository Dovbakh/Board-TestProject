using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    public class UserConfirmEmailRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
