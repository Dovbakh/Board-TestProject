using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    /// <summary>
    /// Категория.
    /// </summary>
    public class Category
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

        /// <summary>
        /// Родительская категория.
        /// </summary>
        public Category? Parent { get; set; }

        /// <summary>
        /// Коллекция подкатегорий.
        /// </summary>
        public ICollection<Category> Children { get; set; }

        /// <summary>
        /// Коллекция обьявлений в категории.
        /// </summary>
        public ICollection<Advert> Adverts { get; set; }

        /// <summary>
        /// Статус категории.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата создания категории.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
