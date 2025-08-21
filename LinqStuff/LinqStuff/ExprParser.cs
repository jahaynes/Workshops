using System.Collections.Immutable;

namespace LinqStuff;

public static class ExprParser
{
    public static Parser<char> ParseCharPred(Func<char, bool> pred)
    {
        return new Parser<char>(Run);

        IEither<string, Tuple<char, string>> Run(string s)
        {
            if (s.Length < 1)
            {
                return IEither<string, Tuple<char, string>>.Left("Out of input");
            }

            var c = s[0];
            if (pred(c))
            {
                return IEither<string, Tuple<char, string>>.Right(new Tuple<char, string>(c, s[1..]));
            }

            return IEither<string, Tuple<char, string>>.Left("Mismatch");
        }
    }

    public static Parser<char> ParseChar(char c) =>
        ParseCharPred(x => x == c);

    public static Parser<ImmutableList<TA>> Many<TA>(Parser<TA> p)
    {
        return new Parser<ImmutableList<TA>>(Run);

        IEither<string, Tuple<ImmutableList<TA>, string>> Run(string s)
        {
            switch (p.Run(s))
            {
                case Left<string, Tuple<TA, string>>:
                    return new Right<string, Tuple<ImmutableList<TA>, string>>(
                        new Tuple<ImmutableList<TA>, string>([], s));

                case Right<string, Tuple<TA, string>> right:
                {
                    var x = right.R.Item1;
                    var ss = right.R.Item2;
                    return Many(p).Run(ss).Select(t =>
                        new Tuple<ImmutableList<TA>, string>(t.Item1.Prepend(x).ToImmutableList(), t.Item2));
                }
                default:
                    throw new Exception("bad");
            }
        }
    }
    
    public static Parser<int> Expr()
    {
        throw new NotImplementedException();
    }

    // number = tokeniser <$> some (charp isDigit)

    // bracketing = char '(' *> expr tokeniser folder <* char ')'
    public static Parser<int> Bracketing() =>
        ParseChar('(')
            .SelectMany(_ => Expr())
            .SelectMany(expr => ParseChar(')').Select(_ => expr));
}