
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Add this line
using Microsoft.Extensions.Configuration.Json; 
using _5unSystem.Model.Entities;
using System.IO; // Add this line


namespace _5unSystem.Core
{
    public class DataContext : DbContext
    {

        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("Main");
                optionsBuilder.UseSqlServer(connectionString);
            }


        }


        public DbSet<User> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<RoleMenu> RoleMenu { get;set; }

    }

}