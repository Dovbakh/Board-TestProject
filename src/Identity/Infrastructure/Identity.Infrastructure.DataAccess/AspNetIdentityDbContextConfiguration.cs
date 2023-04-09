﻿using Identity.Infrastructure.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.DataAccess
{
    public class AspNetIdentityDbContextConfiguration : IDbContextOptionsConfigurator<AspNetIdentityDbContext>
    {
        private const string PostgesConnectionStringName = "PostgresBoardDb";
        private readonly IConfiguration _configuration;
        //private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ShoppingCartContextConfiguration"/>.
        /// </summary>
        /// <param name="configuration">Конфигурация.</param>
        /// <param name="loggerFactory">Фабрика средства логирования.</param>
        public AspNetIdentityDbContextConfiguration(/*ILoggerFactory loggerFactory, */IConfiguration configuration)
        {
            //_loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder<AspNetIdentityDbContext> options)
        {
            var connectionString = _configuration.GetConnectionString(PostgesConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"Не найдена строка подключения с именем '{PostgesConnectionStringName}'");
            }
            options.UseNpgsql(connectionString);

            //options.UseLoggerFactory(_loggerFactory);
        }
    }
}