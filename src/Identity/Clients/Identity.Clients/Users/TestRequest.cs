using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Clients.Users
{
    public class TestRequest
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string scope { get; set; }
        public string client_id { get; set; }
        
    }
}
