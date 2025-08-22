using System.Collections.Immutable;
using static LinqStuff.Parser.ParserExtensions;

namespace LinqStuff.Parser;

public static class Combinators
{
    public static Parser<ImmutableList<TA>> Many<TA>(Parser<TA> p)
    {
        return ManyAcc(ImmutableList<TA>.Empty, p).Select(res => res.Reverse());

        static Parser<ImmutableList<TA>> ManyAcc(ImmutableList<TA> acc, Parser<TA> p) =>
            p.SelectMany(x => ManyAcc(acc.Prepend(x).ToImmutableList(), p))
                .OrElse(Pure(acc));
    }


    public static Parser<ImmutableList<TA>> Some<TA>(Parser<TA> p) =>
        p.SelectMany(first =>
            Many(p).Select(rest => rest.Prepend(first).ToImmutableList()));
}


/*
    Notes:

        * Why is Prepend dumb?

        * Notice we don't do any "Parsing" inside ParseMany.
            * We are only "SelectMany-ing"

 */