using Identity.Infrastructure.DataAccess;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Migrations.Contexts
{
    public class MigrationAspNetIdentityDbContext : AspNetIdentityDbContext
    {

        public MigrationAspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options) : base(options)
        {

        }

    }

}
