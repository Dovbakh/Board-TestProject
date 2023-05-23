using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.DataAccess.Interfaces
{
    /// <summary>
    /// Конфигуратор контекста.
    /// </summary>
    public interface IDbContextOptionsConfigurator<TContext> where TContext : DbContext
    {
        /// <summary>
        /// Выполняет конфигурацию контекста.
        /// </summary>
        /// <param name="options">настройки.</param>
        void Configure(DbContextOptionsBuilder<TContext> options);
    }
}
