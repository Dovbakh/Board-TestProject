using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Users
{
    public class UserChangeEmailRequest
    {
        public string CurrentEmail { get; set; }
        public string NewEmail { get; set; }
        public string Token { get; set; }
    }
}
