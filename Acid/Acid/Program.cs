using Acid.Actions;
using Acid.Db;

namespace Acid;

public static class Program
{
    private const int NumAccounts = 10;
    private const int NumIters = 20;
    private const int ParallelWorkers = 4;
    
    public static async Task Main()
    {
        // await using (var ctx = new MyDbContext()) await Prefill(ctx);
        // await Task.Delay(200);
        await Run();
    }

    private static async Task Prefill(MyDbContext dbContext)
    {
        await new Prefill(dbContext).Run(NumAccounts);
    }

    private static async Task Run()
    {
        await Task.WhenAll(Enumerable.Range(1, ParallelWorkers).Select(_ => Step()));
        return;

        async Task Step()
        {
            for (var i = 0; i < NumIters; i++)
            {
                await using var ctx = new MyDbContext();
                // await new RedistributeWealthSql(ctx).Run(Queries.SQL_READ_COMMITTED);
                await new RedistributeWealthEf(ctx).Run();
            }
        }
    }
}
