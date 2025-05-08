using Acid.Config;
using Acid.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acid.Code;

public class MyDbContext : DbContext
{
    public DbSet<Account> MyAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Db.Conn);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasIndex(u => u.Balance)
            .HasDatabaseName("IX_Account_Balance");
    }
}