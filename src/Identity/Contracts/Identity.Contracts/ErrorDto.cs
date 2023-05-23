using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts
{
    public class ErrorDto
    {
        public string TraceId { get; set; }
        public string Message { get; set; }
    }
}
