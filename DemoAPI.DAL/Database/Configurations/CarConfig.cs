using DemoAPI.Domain.Models;
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
            builder.Property(c => c.HorsePower)
                .IsRequired();
            builder.Property(c => c.IsNew)
                .HasDefaultValue(true);
            #endregion

            #region Keys
            builder.HasKey(c => c.Id);
            #endregion

            #region Relations
            // https://learn.microsoft.com/en-us/ef/core/modeling/relationships
            // Relation 1:N entre Car et Utilisateur (Une voiture peut possèder un propriétaire et un utilisateur peut possèder plusieurs voitures)
            builder.HasOne(c => c.Owner) // Car possède un Owner
                .WithMany(u => u.Cars) // Utilisateur possède plusieurs Cars
                .HasForeignKey("OwnerId") // La clé étrangère est OwnerId dans la DB (on doit le préciser car on fait du shadowing)
                .OnDelete(DeleteBehavior.SetNull);


            // Relation N:N entre Car et CarTag
            builder.HasMany(c => c.Tags)
                .WithMany(t => t.Cars);

                // FULL controlle de la table intermédiaire
                //.UsingEntity("MM_Car_Tag",
                //    tag => tag.HasOne(typeof(CarTag)).WithMany().HasForeignKey("TagId").HasPrincipalKey(nameof(CarTag.Id)).OnDelete(DeleteBehavior.ClientSetNull),
                //    car => car.HasOne(typeof(Car)).WithMany().HasForeignKey("CarId").HasPrincipalKey(nameof(Car.Id)),
                //    join => join.HasKey("TagId", "CarId")
                //);
            #endregion
        }
    }
}
