using Microsoft.EntityFrameworkCore;
using test.Api.Entities;

namespace test.Api;

public class DataContext : DbContext
{
    public DbSet<UserTomelist> UsersInTomelist { get; set; }
    public DbSet<UserWaitlist> UsersInWaitlist { get; set; }

    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        const string localConnectionString =
            "Server=localhost;Port=3306;Database=ogybotdb;Uid=root;Pwd=1234";
        
        const string railwayConnectionString =
            "Server=monorail.proxy.rlwy.net;Port=37441;Database=railway;Uid=root;Pwd=isttWVKUfJaELPJEYdxJaNzOZHMmCUjF;";
        optionsBuilder.UseMySQL(railwayConnectionString);
    }
}