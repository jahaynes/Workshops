using Acid.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acid.Db;

public class MyDbContext : DbContext
{
    public DbSet<Account> MyAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Config.Conn);
        
        // Enable SQL logging
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
                     .EnableSensitiveDataLogging()
                     .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasIndex(u => u.Balance)
            .HasDatabaseName("IX_Account_Balance");
    }
}
