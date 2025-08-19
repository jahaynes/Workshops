using System.Data;
using Acid.Db;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Acid.Actions;

public class RedistributeWealthEf
{
    private readonly MyDbContext _dbContext;

    public RedistributeWealthEf(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Run()
    {
        var done = false;
        while (!done)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                // Find the highest balance
                var maxAccount =
                    await _dbContext.MyAccounts
                        .OrderByDescending(x => x.Balance)
                        .FirstAsync();

                // Choose some other recipients
                var recipient1 =
                    await _dbContext.MyAccounts
                        .Where(a => a.Id != maxAccount.Id)
                        .OrderBy(_ => Guid.NewGuid())
                        .FirstAsync();

                var recipient2 =
                    await _dbContext.MyAccounts
                        .Where(a => a.Id != maxAccount.Id && a.Id != recipient1.Id)
                        .OrderBy(_ => Guid.NewGuid())
                        .FirstAsync();

                // Move one half
                var onePart = maxAccount.Balance / 2;
                maxAccount.Balance -= onePart;
                recipient1.Balance += onePart;

                // Move the other half
                var otherPart = maxAccount.Balance - onePart;
                maxAccount.Balance -= otherPart;
                recipient2.Balance += otherPart;

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                done = true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 1205 })
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Deadlocked. Will try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}