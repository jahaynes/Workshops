namespace Acid.Code;

public static class Program
{
    public static async Task Main()
    {
        //await using var accountOps = new AccountOps();
        //await accountOps.Prefill(10);

        await Task.WhenAll(
            Enumerable
                .Range(1, 2)
                .AsParallel()
                .Select(_ => Task.Run(Foo))
        );
    }

    private static async Task Foo()
    {
        await using var accountOps = new AccountOps();

        for (var i = 0; i < 100; i++)
        {
            await accountOps.RedistributeWealth();
        }
    }
}