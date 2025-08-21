namespace LinqStuff;

public interface IEither<TL, TR>
{
    public static Left<TL, TR> Left(TL l) => new(l);
    public static Right<TL, TR> Right(TR r) => new(r);
}

public record struct Left<TL, TR>(TL L) : IEither<TL, TR>;

public record struct Right<TL, TR>(TR R) : IEither<TL, TR>;