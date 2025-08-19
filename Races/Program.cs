namespace Races;

public static class Program
{
    private const int NumThreads = 2;
    private const int NumIterations = 1000;

    private static Object myLock = new Object();
    
    public static void Main()
    {
        var successes = 0;
        while (true)
        {
            var dekker = new Dekker2();

            var count = 0;

            runOnMultipleThreads(NumThreads, NumIterations,
                threadId => { dekker.critical_section(threadId, _ =>
                {
                    lock (myLock) {
                        count++;
                    }
                }); });

            if (count != NumThreads * NumIterations)
            {
                throw new Exception($"Failed after {successes} successes");
            }

            successes++;
        }
    }

    private static void runOnMultipleThreads(int numThreads, int numIterations, Action<int> action)
    {
        var threads = new Thread[numThreads];

        for (var t = 0; t < numThreads; t++)
        {
            var threadId = t;
            threads[threadId] = new Thread(() =>
            {
                for (var i = 0; i < numIterations; i++)
                {
                    action(threadId);
                }
            });
            threads[t].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}

class Dekker2
{
    private volatile bool[] _wanted = [false, false];
    private volatile int _turn = 1;

    public void critical_section(int threadId, Action<int> action)
    {
        var myTurn = threadId + 1;
        var otherTurn = 2 - threadId;
        var myWant = threadId;
        var otherWant = 1 - threadId;

        _wanted[myWant] = true;

        while (_wanted[otherWant])
        {
            if (_turn == otherTurn)
            {
                _wanted[myWant] = false;
                while (_turn != myTurn)
                {
                    // busy wait
                }

                _wanted[myWant] = true;
            }
        }

        action(threadId);

        _turn = otherTurn;
        _wanted[myWant] = false;
    }
}