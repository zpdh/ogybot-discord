using System.Configuration;
using Microsoft.EntityFrameworkCore;
using test.Services;

namespace test.Api;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DataContext() {}
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL(ConfigurationManager.AppSettings["ConnectionString"]);
        }
    }
}