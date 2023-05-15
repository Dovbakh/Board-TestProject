using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.AdvertImages
{
    public class AdvertImageAddRequest
    {
        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid AdvertId { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid ImageId { get; set; }
    }
}
