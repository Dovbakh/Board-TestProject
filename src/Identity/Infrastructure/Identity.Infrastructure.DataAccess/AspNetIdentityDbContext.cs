﻿using Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.DataAccess
{
    public class AspNetIdentityDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options) 
            : base(options) 
        { 

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            base.OnModelCreating(modelBuilder);
        }
    }
}
