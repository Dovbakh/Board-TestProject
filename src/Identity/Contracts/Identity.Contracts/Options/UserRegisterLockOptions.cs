using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Options
{
    public class UserRegisterLockOptions
    {
        public string UserRegisterKey { get; set; } = "UserRegisterKey_";
        public TimeSpan Expire { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan Wait { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan Retry { get; set; } = TimeSpan.FromSeconds(1);
    }
}
