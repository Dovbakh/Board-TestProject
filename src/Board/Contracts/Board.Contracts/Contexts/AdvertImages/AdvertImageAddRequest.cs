using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.AdvertImages
{
    /// <summary>
    /// Модель добавления записи отношения изображения к обьявлению.
    /// </summary>
    public class AdvertImageAddRequest
    {
        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid AdvertId { get; set; }

        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid ImageId { get; set; }
    }
}
