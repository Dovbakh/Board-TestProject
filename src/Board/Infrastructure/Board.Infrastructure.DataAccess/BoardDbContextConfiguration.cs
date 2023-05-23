using Board.Infrastructure.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess
{
    /// <inheritdoc />
    public class BoardDbContextConfiguration : IDbContextOptionsConfigurator<BoardDbContext>
    {
        private const string PostgesConnectionStringName = "PostgresBoardDb";
        private readonly IConfiguration _configuration;

        public BoardDbContextConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder<BoardDbContext> options)
        {
            var connectionString = _configuration.GetConnectionString(PostgesConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"Не найдена строка подключения с именем '{PostgesConnectionStringName}'");
            }
            options.UseNpgsql(connectionString);
        }
    }
}
