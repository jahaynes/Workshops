using LinqStuff.Either;

namespace LinqStuff.Parser;

public static class Strings
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
}