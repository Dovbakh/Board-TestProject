using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.UserImages.Configurations
{
    public class UserImageConfiguration : IEntityTypeConfiguration<Domain.UserImage>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.UserImage> builder)
        {
            builder.ToTable("UserImages");


            builder.HasKey(ai => ai.Id);
            builder.Property(ai => ai.Id).ValueGeneratedOnAdd();

        }
    }
}
