using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.PostImages.Configurations
{
    /// <summary>
    /// Конфигурация таблицы PostImages
    /// </summary>
    public class AdvertisementImageConfiguration : IEntityTypeConfiguration<Domain.PostImage>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.PostImage> builder)
        {
            builder.ToTable("PostImages");

            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.Id).ValueGeneratedOnAdd();

        }
    }
}
