using DemoAPI.Domain.CustomEnums;
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
    public class UtilisateurConfig : IEntityTypeConfiguration<Utilisateur>
    {
        public void Configure(EntityTypeBuilder<Utilisateur> builder)
        {
            builder.ToTable("Utilisateurs");

            #region Properties
            builder.Property(u => u.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(30);
            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(u => u.Grade)
                .IsRequired()
                .HasConversion(
                    g => g.ToString(),
                    g => (Grade)Enum.Parse(typeof(Grade), g)
                );
            #endregion

            #region Keys
            builder.HasKey(u => u.Id)
                .HasName("PK_Utilisateur");
            #endregion
        }
    }
}
