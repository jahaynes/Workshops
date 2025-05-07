using Acid.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acid.Code;

public class AccountOps : IAsyncDisposable
{
    private readonly Random _rng = new();
    private readonly MyDbContext _dbContext = new();

    public async Task Prefill(int n)
    {
        for (var i = 0; i < n; i++)
        {
            _dbContext.MyAccounts.Add(new Account { Balance = _rng.Next() % 1000 });
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task RedistributeWealth()
    {
        await using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

        // Find the highest balance
        var maxAccount =
            await _dbContext.MyAccounts.OrderByDescending(x => x.Balance).FirstAsync();

        // Choose some other recipients
        var recipient1 =
            await _dbContext.MyAccounts
                .Where(a => a.Balance != maxAccount.Balance)
                .OrderBy(_ => Guid.NewGuid())
                .FirstAsync();

        var recipient2 =
            await _dbContext.MyAccounts
                .Where(a => a.Balance != maxAccount.Balance)
                .OrderBy(_ => Guid.NewGuid())
                .FirstAsync();

        // Split the balance in two
        var onePart = maxAccount.Balance / 2;
        var otherPart = maxAccount.Balance - onePart;

        // Move one half
        maxAccount.Balance -= onePart;
        recipient1.Balance += onePart;

        // Move the other half
        maxAccount.Balance -= otherPart;
        recipient2.Balance += otherPart;

        await _dbContext.SaveChangesAsync();
        await dbContextTransaction.CommitAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}