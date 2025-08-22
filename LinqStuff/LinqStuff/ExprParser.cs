using System.Collections.Immutable;
using LinqStuff.Parser;
using static LinqStuff.Parser.Combinators;
using static LinqStuff.Parser.Strings;

namespace LinqStuff;

public static class ExprParser
{
    public static Parser<int> Expr() =>
        Addition;

    public static Parser<int> Addition =>
        Multiplication.SelectMany(first =>
            Many(Plus.OrElse(Minus))
                .Select(rest => Collapse(first, rest)));

    public static Parser<Tuple<Op, int>> Plus =>
        ParseChar('+').SelectMany(_ => Multiplication.Select(term => new Tuple<Op, int>(Op.Plus, term)));

    public static Parser<Tuple<Op, int>> Minus =>
        ParseChar('-').SelectMany(_ => Multiplication.Select(term => new Tuple<Op, int>(Op.Minus, term)));

    public static Parser<int> Multiplication =
        Term.SelectMany(first =>
            Many(Times.OrElse(Divide))
                .Select(rest => Collapse(first, rest)));

    public static Parser<Tuple<Op, int>> Times =
        ParseChar('*').SelectMany(_ => Multiplication.Select(term => new Tuple<Op, int>(Op.Mul, term)));

    public static Parser<Tuple<Op, int>> Divide =
        ParseChar('/').SelectMany(_ => Multiplication.Select(term => new Tuple<Op, int>(Op.Div, term)));

    public static Parser<int> Term =>
        Number.OrElse(Bracketing);

    public static Parser<int> Number =>
        Some(ParseCharPred(c => c is >= '0' and <= '9'))
            .Select(x => int.Parse(new string(x.ToArray())));

    public static Parser<int> Bracketing =>
        ParseChar('(')
            .SelectMany(_ => Expr())
            .SelectMany(expr => ParseChar(')').Select(_ => expr));

    private static int Collapse(int first, ImmutableList<Tuple<Op, int>> rest) =>
        rest.Aggregate(first, (acc, b) =>
            b.Item1 switch
            {
                Op.Plus => acc + b.Item2,
                Op.Minus => acc - b.Item2,
                Op.Mul => acc * b.Item2,
                Op.Div => acc / b.Item2,
                _ => throw new ArgumentOutOfRangeException()
            }
        );

    public enum Op
    {
        Plus,
        Minus,
        Mul,
        Div
    }
}