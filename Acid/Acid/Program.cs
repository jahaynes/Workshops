using Acid.Actions;
using Acid.Db;

namespace Acid;

public static class Program
{
    public static async Task Main()
    {
        // await using (var ctx = new MyDbContext()) await Prefill(ctx);
        await Task.Delay(1000);
        await Run();
    }

    private static async Task Prefill(MyDbContext dbContext)
    {
        await new Prefill(dbContext).Run(10);
    }

    private static async Task Run()
    {
        await Task.WhenAll(Enumerable.Range(1, 4).Select(_ => Step()));
        return;

        async Task Step()
        {
            for (var i = 0; i < 5; i++)
            {
                await using var ctx = new MyDbContext();
                // await new RedistributeWealthSql(ctx).Run(Queries.SQL_READ_COMMITTED);
                await new RedistributeWealthEf(ctx).Run();
            }
        }
    }
}