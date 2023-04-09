using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Users.Configurations
{
    /// <summary>
    /// Конфигурация таблицы Users.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<Domain.User>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.Name).HasMaxLength(100);
            builder.Property(u => u.Address).HasMaxLength(500);


            builder.HasMany(u => u.Adverts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.CommentsBy)
                .WithOne(c => c.Author)
                .HasForeignKey(c => c.AuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.CommentsFor)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
