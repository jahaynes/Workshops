using System.Collections.Immutable;
using LinqStuff.Either;
using static LinqStuff.ExprParser;
using static LinqStuff.Parser.Strings;
using static LinqStuff.Parser.Combinators;

namespace LinqStuff;

public static class Program
{
    private static void Main(string[] args)
    {
        //var number = Number.Run("123");


        var parser = Term;

        var input = "(123)";

        Console.WriteLine(parser.Run(input));
    }
}