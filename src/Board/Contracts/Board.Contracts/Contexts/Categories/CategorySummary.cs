using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Categories
{
    /// <summary>
    /// Модель с краткой информацией о категории.
    /// </summary>
    public class CategorySummary
    {
        /// <summary>
        /// Идентификатор категории.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название категории.
        /// </summary>  
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор родительской категории.
        /// </summary>
        public Guid? ParentId { get; set; }
    }
}
