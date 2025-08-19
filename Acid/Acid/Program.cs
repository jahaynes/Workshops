using System.Data;
using Acid.Actions;
using Acid.Db;
using Acid.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acid;

public static class Program
{
    private const int NumAccounts = 6;
    private const int NumThreads = 4;

    public static async Task Main()
    {
        await Prefill();
        await Run();
    }

    private static async Task Prefill()
    {
        await using var dbContext = new MyDbContext();
        var initialAccounts = await new Prefill(dbContext).Run(NumAccounts);
        SummariseAccounts(initialAccounts);
    }

    private static async Task FetchAndSummariseAccounts()
    {
        await using var dbContext = new MyDbContext();
        var list = await dbContext.MyAccounts.ToListAsync();
        SummariseAccounts(list);
    }

    private static void SummariseAccounts(List<Account> accounts)
    {
        var balances = string.Join(", ", accounts.Select(a => a.Balance));
        var total = accounts.Select(a => a.Balance).Sum();
        Console.WriteLine($"Created {accounts.Count} accounts, balances: {balances}");
        Console.WriteLine($"The total is: {total}");
    }

    private static async Task Run()
    {
        Console.WriteLine($"Setting up {NumThreads} parallel connections");
        var conns = Enumerable
            .Range(1, NumThreads)
            .AsParallel()
            .Select(_ => new MyDbContext())
            .ToList();

        Console.WriteLine("Setting application layer");
        // var businessLogics = conns.Select(conn => new RedistributeWealthEf(conn));
        var businessLogics = conns.Select(conn => new RedistributeWealthSql(conn));
        
        Console.WriteLine("Running application");
        await Parallel.ForEachAsync(businessLogics,
            async (logic, _) => await logic.Run(Queries.SQL_TRANS_SERIALIZABLE));

        Console.WriteLine("Shutting down");
        foreach (var ctx in conns)
        {
            await ctx.DisposeAsync();
        }

        Console.WriteLine("Checking");
        await FetchAndSummariseAccounts();

        Console.WriteLine("Done");
    }
}