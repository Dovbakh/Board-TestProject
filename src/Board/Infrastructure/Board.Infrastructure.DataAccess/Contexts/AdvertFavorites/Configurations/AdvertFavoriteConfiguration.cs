using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.AdvertFavorites.Configurations
{
    public class AdvertFavoriteConfiguration : IEntityTypeConfiguration<Domain.AdvertFavorite>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.AdvertFavorite> builder)
        {
            builder.ToTable("AdvertFavorites");


            builder.HasKey(ai => ai.Id);
            builder.Property(ai => ai.Id).ValueGeneratedOnAdd();

        }
    }
}
