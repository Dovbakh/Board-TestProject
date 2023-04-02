using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Adverts.Configurations
{
    /// <summary>
    /// Конфигурация таблицы Posts.
    /// </summary>
    public class AdvertConfiguration : IEntityTypeConfiguration<Domain.Advert>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.Advert> builder)
        {
            builder.ToTable("Adverts");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.Name).HasMaxLength(50);

            builder.Property(a => a.Description).HasMaxLength(2000);

            builder.Property(a => a.Address).HasMaxLength(2000);

            builder.Property(a => a.Phone).HasMaxLength(50);

            builder.Property(a => a.UserName).HasMaxLength(100);

            builder.Property(a => a.CreatedAt).HasConversion(d => d, d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

            builder.HasMany(a => a.AdvertImages)
                .WithOne(ai => ai.Advert)
                .HasForeignKey(ai => ai.AdvertId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
