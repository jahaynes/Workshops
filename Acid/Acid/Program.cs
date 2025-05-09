using Acid.Actions;
using Acid.Db;

namespace Acid;

public static class Program
{
    public static async Task Main()
    {
        await Prefill();
    }

    private static async Task Prefill()
    {
        await using var dbContext = new MyDbContext();
        await new Prefill(dbContext).Run(6);
    }

    private static async Task Run()
    {
        await using var dbContext = new MyDbContext();
        
        var redistributeWealthSql = new RedistributeWealthSql(dbContext);
        var redistributeWealthEf = new RedistributeWealthEf(dbContext);

        await Task.WhenAll(
            Enumerable
                .Range(1, 1)
                .AsParallel()
                .Select(_ => Step())
        );
        return;

        async Task Step()
        {
            for (var i = 0; i < 10; i++)
            {
                await redistributeWealthSql.Run(Queries.SQL_READ_COMMITTED);
            }
        }
    }
}