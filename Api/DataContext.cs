using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using test.Services;

namespace test.Api;

public class DataContext : DbContext
{
    public DbSet<UserTomelist> UsersInTomelist { get; set; }
    public DbSet<UserWaitlist> UsersWaitlists { get; set; }

    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        const string connectionString =
            "Server=monorail.proxy.rlwy.net;Port=37441;Database=railway;Uid=root;Pwd=isttWVKUfJaELPJEYdxJaNzOZHMmCUjF;";
        optionsBuilder.UseMySQL(connectionString);
    }
}