using System.Data;
using Acid.Db;
using Microsoft.EntityFrameworkCore;

namespace Acid.Actions;

public class RedistributeWealthEf
{
    private readonly MyDbContext _dbContext;

    public RedistributeWealthEf(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Run(IsolationLevel? isolationLevel)
    {
        if (isolationLevel is null)
        {
            try
            {
                await Run_();
            }
            catch (Exception)
            {
                Console.WriteLine("Failure!");
            }
        }
        else
        {
            var success = false;
            while (!success)
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel.Value);
                try
                {
                    await Run_();
                    await transaction.CommitAsync();
                    Console.WriteLine("Success!");
                    success = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Failure... retrying!");
                }
            }
        }
    }

    private async Task Run_()
    {
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
    }
}