using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class AdvertImage
    {
        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid AdvertId { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Дата загрузки изображения.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Обьявление, к которому относится изображение.
        /// </summary>
        public Advert Advert { get; set; }

        public bool isActive { get; set; }
    }
}
