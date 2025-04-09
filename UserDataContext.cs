using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;


namespace studentoo
{
    
    public class UserDataContext : DbContext
    {
        public DbSet<paired> paireds {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<photos> photos { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); 
                entity.Property(e => e.login).HasMaxLength(50);
                entity.HasIndex(e => e.id).IsUnique();
            });
        }
    }
}