namespace LinqStuff;

public static class Rando
{
    public static IEither<string, int> SafeDiv(int a, int b) =>
        b switch
        {
            0 => new Left<string, int>("Divide by zero"),
            _ => new Right<string, int>(a / b),
        };
}