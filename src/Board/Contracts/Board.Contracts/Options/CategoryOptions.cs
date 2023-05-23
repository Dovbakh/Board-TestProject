using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции сервиса для работы с категориями.
    /// </summary>
    public class CategoryOptions
    {
        /// <summary>
        /// Ключ распределенного кэша, содержащего список категорий.
        /// </summary>
        public string CategoryListKey { get; set; } = "CategoryListKey_";
    }
}
