using Acid.Db;
using Acid.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acid.Actions;

public class Prefill
{
    private readonly Random _rng = new();
    private readonly MyDbContext _dbContext;

    public Prefill(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Account>> Run(int n)
    {

        await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE MyAccounts");

        for (var i = 0; i < n; i++)
        {
            _dbContext.MyAccounts.Add(new Account { Balance = _rng.Next() % 1000 });
        }

        await _dbContext.SaveChangesAsync();

        return await _dbContext.MyAccounts.ToListAsync();
    }
}