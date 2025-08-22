using LinqStuff.Parser;
using static LinqStuff.Parser.Strings;

namespace LinqStuff;

public static class ExprParser
{
    public static Parser<int> Expr()
    {
        throw new NotImplementedException();
    }

    // term = number <|> bracketing
    public static Parser<int> Term =>
        Number.OrElse(Bracketing);

    // number = some (charp isDigit)
    public static Parser<int> Number =>
        Combinators
            .Some(ParseCharPred(c => c is >= '0' and <= '9'))
            .Select(x => int.Parse(new string(x.ToArray())));

    // bracketing = char '(' *> expr <* char ')'
    public static Parser<int> Bracketing =>
        ParseChar('(')
            .SelectMany(_ => Expr())
            .SelectMany(expr => ParseChar(')').Select(_ => expr));
}