using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.AdvertViews.Configurations
{
    public class AdvertViewConfiguration : IEntityTypeConfiguration<Domain.AdvertView>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Domain.AdvertView> builder)
        {
            builder.ToTable("AdvertViews");


            builder.HasKey(ai => ai.Id);
            builder.Property(ai => ai.Id).ValueGeneratedOnAdd();

        }
    }
}
