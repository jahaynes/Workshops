using static LinqStuff.ExprParser;

namespace LinqStuff;

public static class Program
{
    private static void Main()
    {
        var parser = Expr();

        var input = "(1 + 2 * 3 - 4) / 2".Replace(" ", "");

        Console.WriteLine(parser.Run(input));
    }
}