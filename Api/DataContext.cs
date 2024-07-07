using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using test.Services;

namespace test.Api;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DataContext() : base()
    {
    }
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL("Server=monorail.proxy.rlwy.net;Port=37441;Database=railway;Uid=root;Pwd=isttWVKUfJaELPJEYdxJaNzOZHMmCUjF;");
        }
    }
}