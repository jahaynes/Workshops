namespace LinqStuff;

public record struct Parser<TA>(Func<string, IEither<string, Tuple<TA, string>>> Run);