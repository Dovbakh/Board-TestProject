using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции сервиса для работы с избранными обьявлениями.
    /// </summary>
    public class AdvertFavoriteOptions
    {
        /// <summary>
        /// Количество получаемых обьявлений по умолчанию.
        /// </summary>
        public int ListDefaultCount { get; set; } = 10;
    }
}
