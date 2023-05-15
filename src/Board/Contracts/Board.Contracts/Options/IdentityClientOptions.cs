using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    public class IdentityClientOptions
    {
        public string GetTokenAddress { get; set; }
        public ExternalClientCredentials ExternalClientCredentials { get; set; }
        public InternalClientCredentials InternalClientCredentials { get; set; }
        public ApiResourseCredentials ApiResourseCredentials { get; set; }
    }

    public class ExternalClientCredentials
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }

    public class InternalClientCredentials
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }

    public class ApiResourseCredentials
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }
}