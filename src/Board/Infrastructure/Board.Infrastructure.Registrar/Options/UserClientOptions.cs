using Board.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.Options
{
    public class UserClientOptions
    {
        public string BasePath { get; set; }
    }
}
