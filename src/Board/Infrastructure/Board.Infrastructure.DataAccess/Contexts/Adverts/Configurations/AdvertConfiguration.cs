using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Posts.Configurations
{
    /// <summary>
    /// Конфигурация таблицы Posts.
    /// </summary>
    public class AdvertConfiguration : IEntityTypeConfiguration<Domain.Advert>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.Advert> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Name).HasMaxLength(50);

            builder.Property(p => p.Description).HasMaxLength(2000);

            builder.Property(p => p.Address).HasMaxLength(2000);

            builder.Property(p => p.Phone).HasMaxLength(50);

            builder.Property(p => p.UserName).HasMaxLength(100);

            builder.HasMany(p => p.PostImages)
                .WithOne(pi => pi.Post)
                .HasForeignKey(pi => pi.PostId);
        }
    }
}
