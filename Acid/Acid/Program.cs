using System.Data;
using Acid.Actions;
using Acid.Db;

namespace Acid;

public static class Program
{
    private const int NumAccounts = 6;
    private const int NumThreads = 4;

    public static async Task Main()
    {
        // await Prefill();
        await Run();
    }

    private static async Task Prefill()
    {
        await using var dbContext = new MyDbContext();
        await new Prefill(dbContext).Run(NumAccounts);
    }

    private static async Task Run()
    {
        Console.WriteLine("Setting up db connections");
        var conns = Enumerable
            .Range(1, NumThreads)
            .AsParallel()
            .Select(_ => new MyDbContext())
            .ToList();

        Console.WriteLine("Setting application layer");
        var businessLogics = conns.Select(conn => new RedistributeWealthEf(conn));

        Console.WriteLine("Running application");
        await Parallel.ForEachAsync(businessLogics,
            async (logic, _) =>
                await logic.Run(IsolationLevel.RepeatableRead));

        Console.WriteLine("Shutting down");
        foreach (var ctx in conns)
        {
            await ctx.DisposeAsync();
        }

        Console.WriteLine("Done");
    }
}