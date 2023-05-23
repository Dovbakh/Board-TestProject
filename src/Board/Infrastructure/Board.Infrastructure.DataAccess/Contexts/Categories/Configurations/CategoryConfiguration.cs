using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Categories.Configurations
{
    /// <summary>
    /// Конфигурация таблицы Categories.
    /// </summary>
    public class CategoryConfiguration : IEntityTypeConfiguration<Domain.Category>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();


            builder.Property(c => c.Name).HasMaxLength(100);

            builder.HasMany(с => с.Adverts)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId);

            builder.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
