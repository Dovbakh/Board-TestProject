using Board.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Migrations
{
    public class MigrationsDbContext : BoardDbContext
    {
        public MigrationsDbContext(DbContextOptions<MigrationsDbContext> options) : base(options)
        {
        }
    }
}
