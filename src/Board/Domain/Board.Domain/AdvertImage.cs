using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    /// <summary>
    /// Запись отношения изображения к обьявлению.
    /// </summary>
    public class AdvertImage
    {
        /// <summary>
        /// Идентификатор записи..
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid AdvertId { get; set; }

        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Дата создания записи.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Обьявление, к которому относится изображение.
        /// </summary>
        public Advert Advert { get; set; }

        /// <summary>
        /// Статус записи.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
