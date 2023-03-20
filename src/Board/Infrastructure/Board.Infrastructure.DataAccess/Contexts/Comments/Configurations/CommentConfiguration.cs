using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Comments.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Domain.Comment>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Text).HasMaxLength(1000);

        }
    }
}
