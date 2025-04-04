using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace studentoo
{
    public class UserDataContext : DbContext
    {
       
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            string connectionString =
                "Server=178.43.79.128,1433;Database=studentooDB;User Id=sa;Password=superadmin;Encrypt=False;";

            optionsBuilder.UseSqlServer(connectionString); 
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