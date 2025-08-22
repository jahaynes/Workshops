using static LinqStuff.Either.IEither<object, object>;
using LinqStuff.Either;

namespace LinqStuff.Examples;

public static class Examples
{
    public static IEither<string, int> SafeDiv(int a, int b) =>
        b switch
        {
            0 => new Left<string, int>("Divide by zero"),
            _ => new Right<string, int>(a / b),
        };


    private static void Shorthand()
    {
        var r = Right(3);
        CaseMatching(r);

        var l = Left("bad");
        CaseMatching(l);

        var x = SafeDiv(8, 2).SelectMany(x => SafeDiv(x, 0));
    }

    private static void CaseMatching<TL, TR>(IEither<TL, TR> either)
    {
        switch (either)
        {
            case Left<TL, TR>(var l):
                Console.WriteLine("This is your left: " + l);
                break;
            case Right<TL, TR>(var r):
                Console.WriteLine("That's your right: " + r);
                break;
        }
    }
}