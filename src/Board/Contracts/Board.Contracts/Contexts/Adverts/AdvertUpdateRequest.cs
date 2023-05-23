using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Adverts
{
    /// <summary>
    /// Модель изменения обьявления.
    /// </summary>
    public class AdvertUpdateRequest
    {
        /// <summary>
        /// Название обьявления.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Текст обьявления.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Цена, указанная в обьявлении.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Адрес, указанный в обьявлении.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Идентификатор категории обьявления.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Список идентификаторов с добавленными изображениями.
        /// </summary>
        public ICollection<Guid> NewImagesId { get; set; }

        /// <summary>
        /// Список идентификаторов с удаленными изображениями.
        /// </summary>
        public ICollection<Guid> RemovedImagesId { get; set; }
    }
}
