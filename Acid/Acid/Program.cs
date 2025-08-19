using Acid.Actions;
using Acid.Db;

namespace Acid;

public static class Program
{
    public static async Task Main()
    {
        // await Prefill();
        await Run();
    }

    private static async Task Prefill()
    {
        await using var dbContext = new MyDbContext();
        await new Prefill(dbContext).Run(6);
    }

    private static async Task Run()
    {
        await Task.WhenAll(
            Enumerable
                .Range(1, 4)
                .AsParallel()
                .Select(_ => Step())
        );
        return;

        async Task Step()
        {
            await using var dbContext = new MyDbContext();
            for (var i = 0; i < 5; i++)
            {
                // await new RedistributeWealthSql(dbContext).Run(Queries.SQL_READ_COMMITTED);
                await new RedistributeWealthEf(dbContext).Run();
            }
        }
    }
}