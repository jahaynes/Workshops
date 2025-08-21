namespace LinqStuff;

public static class ParserExtensions
{
    /*      instance Functor Parser where
               fmap f (Parser run) = Parser $ \s ->
                   case run s of
                       Left l        -> Left l
                       Right (x, s') -> Right (f x, s')
    */

    public static Parser<TB> Select<TA, TB>(
        this Parser<TA> parser,
        Func<TA, TB> f)
    {
        return new Parser<TB>(Run);

        IEither<string, Tuple<TB, string>> Run(string s)
        {
            return parser.Run(s) switch
            {
                Left<string, Tuple<TA, string>> left => new Left<string, Tuple<TB, string>>(left.L),
                Right<string, Tuple<TA, string>> right => new Right<string, Tuple<TB, string>>(
                    new Tuple<TB, string>(f(right.R.Item1), right.R.Item2)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    /* instance Monad Parser where
           return = pure
           Parser run >>= mf = Parser $ \s ->
               case run s of
                   Left l -> Left l
                   Right (x, s') -> let Parser run' = mf x in run' s'
    */
    public static Parser<TB> SelectMany<TA, TB>(
        this Parser<TA> parser,
        Func<TA, Parser<TB>> f)
    {
        return new Parser<TB>(Run);

        IEither<string, Tuple<TB, string>> Run(string s)
        {
            return parser.Run(s) switch
            {
                Left<string, Tuple<TA, string>> left => new Left<string, Tuple<TB, string>>(left.L),
                Right<string, Tuple<TA, string>> right => f(right.R.Item1).Run(right.R.Item2),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public static Parser<TA> OrElse<TA>(
        this Parser<TA> parser,
        Parser<TA> other)
    {
        return new Parser<TA>(Run);

        IEither<string, Tuple<TA, string>> Run(string s)
        {
            return parser.Run(s) switch
            {
                Left<string, Tuple<TA, string>> => other.Run(s),
                Right<string, Tuple<TA, string>> right => right,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}