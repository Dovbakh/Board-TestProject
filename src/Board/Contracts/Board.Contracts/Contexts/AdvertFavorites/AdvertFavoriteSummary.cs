using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.AdvertFavorites
{
    public class AdvertFavoriteSummary
    {
        /// <summary>
        /// Идентификатор избранного обьявления.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid AdvertId { get; set; }

        public Guid UserId { get; set; }
    }
}
