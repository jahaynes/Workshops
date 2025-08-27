using System.Collections.Immutable;

namespace LinqStuff.Existing;

using Parser;
using static Parser.Combinators;
using static Parser.Strings;

public static class ReplacementTokenizer
{
    public static Parser<ImmutableList<string>> CommandLine() =>
        Ws().Then(
            Some(SimpleToken.FollowedBy(Ws())
                .OrElse(QuotedToken.FollowedBy(Ws()))
            )
        );

    private static Parser<string> SimpleToken =>
        Some(SimpleTokenChar).Select(cs => new string(cs.ToArray()));

    private static Parser<char> SimpleTokenChar =>
        SpecialCharPair.OrElse(OrdinaryChar);

    private static Parser<string> QuotedToken =>
        ParseChar('\"').Then(
            Some(QuotedTokenChar).Select(cs => new string(cs.ToArray()))
        ).FollowedBy(
            ParseChar('\"'));

    private static Parser<char> QuotedTokenChar =>
        SpecialCharPair
            .OrElse(OrdinaryChar)
            .OrElse(ParseChar(' '));

    private static Parser<char> SpecialCharPair =>
        ParseChar('\\').Then(
            ParseChar('\\').OrElse(ParseChar('\"'))
        );

    private static Parser<char> OrdinaryChar =>
        ParseCharPred(c => c != '\"' && c != ' ');
}