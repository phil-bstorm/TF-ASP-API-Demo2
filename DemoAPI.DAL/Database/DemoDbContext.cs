using DemoAPI.DAL.Database.Configurations;
using DemoAPI.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.DAL.Database
{
    public class DemoDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configurations des tables...
            modelBuilder.ApplyConfiguration(new CarConfig());
        }
    }
}
