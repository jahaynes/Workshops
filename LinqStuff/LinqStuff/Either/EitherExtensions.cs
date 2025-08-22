namespace LinqStuff.Either;

public static class EitherExtensions
{
    /*      instance Functor (IEither l) where
                fmap _ (Left l)  = Left l
                fmap f (Right r) = Right (f r)
    */

    public static IEither<TL, TO> Select<TL, TR, TO>(
        this IEither<TL, TR> either,
        Func<TR, TO> f)
    {
        return either switch
        {
            Left<TL, TR>(var l) => new Left<TL, TO>(l),
            Right<TL, TR>(var r) => new Right<TL, TO>(f(r)),
            _ => throw new ArgumentOutOfRangeException(nameof(either)),
        };
    }

    /*      instance Monad (IEither l) where
                return = pure
                IRight r >>= f = f r
                ILeft l  >>= _ = ILeft l
    */

    public static IEither<TL, TO> SelectMany<TL, TR, TO>(
        this IEither<TL, TR> either,
        Func<TR, IEither<TL, TO>> f)
    {
        return either switch
        {
            Left<TL, TR>(var l) => new Left<TL, TO>(l),
            Right<TL, TR>(var r) => f(r),
            _ => throw new ArgumentOutOfRangeException(nameof(either)),
        };
    }
}