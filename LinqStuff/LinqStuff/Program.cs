using static LinqStuff.IEither<object, object>;

namespace LinqStuff;

public static class Program
{
    private static void Main(string[] args)
    {
        var r = Right(3);
        CaseMatching(r);

        var l = Left("bad");
        CaseMatching(l);


        //var x = SafeDiv(8, 2).SelectMany(x => SafeDiv(x, 0));

      
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