using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using TorgovayaPloshadka.Models;

namespace TorgovayaPloshadka.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }

        public DbSet<CustomUser> CustomUsers { get; set; }
        public DbSet<Doljnost> Doljnosts { get; set; }

        public DbSet<Order_CountOtchet> Order_CountOtchet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable(tb => tb.HasTrigger("Check_Category"));
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable(tb => tb.HasTrigger("AddDelCount"));
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable(tb => tb.HasTrigger("UpdCount"));
            base.OnModelCreating(modelBuilder);
        }
    }
}