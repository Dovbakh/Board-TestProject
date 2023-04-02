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
    public class AdvertisementImageConfiguration : IEntityTypeConfiguration<Domain.AdvertImage>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.AdvertImage> builder)
        {
            builder.ToTable("AdvertImages");

            builder.Property(ai => ai.CreatedAt).HasConversion(d => d, d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

            builder.HasKey(ai => ai.Id);
            builder.Property(ai => ai.Id).ValueGeneratedOnAdd();

        }
    }
}
