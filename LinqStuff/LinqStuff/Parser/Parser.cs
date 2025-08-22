using LinqStuff.Either;

namespace LinqStuff.Parser;

public record struct Parser<TA>(Func<string, IEither<string, Tuple<TA, string>>> Run);