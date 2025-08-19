using Acid.Db;
using Acid.Entity;

namespace Acid.Actions;

public class Prefill
{
    private readonly Random _rng = new();
    private readonly MyDbContext _dbContext;

    public Prefill(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Run(int n)
    {
        for (var i = 0; i < n; i++)
        {
            _dbContext.MyAccounts.Add(new Account { Balance = _rng.Next() % 1000 });
        }

        await _dbContext.SaveChangesAsync();
    }
}