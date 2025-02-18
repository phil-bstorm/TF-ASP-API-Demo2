using DemoAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.DAL.Database.Configurations
{
    public class CarConfig : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.ToTable("Cars");

            #region Properties
            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Brand)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(c => c.Model)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(c => c.Color)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(c => c.IsNew)
                .HasDefaultValue(true);
            #endregion

            #region Keys
            builder.HasKey(c => c.Id);
            #endregion
        }
    }
}
