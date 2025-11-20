using System;
using Check.Data;
using CsCheck;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Check.Tests.Data;

[TestFixture]
[TestOf(typeof(Personnummer))]
public class PersonnummerTest
{
    [Theory]
    public void Method()
    {
        PsersonnummerTestData.GenTestPair.Array.Sample(x => Console.WriteLine(JsonConvert.SerializeObject(x)));
    }
}

readonly record struct PersonNummerData(int Year, int Month, int Day, int Num);

readonly record struct TestPair(PersonNummerData Data, string Str);

internal static class PsersonnummerTestData
{
    private static readonly Gen<int> GenYear =
        Gen.Int[1947, 2046];

    private static readonly Gen<int> GenMonth =
        Gen.Int[1, 12];

    private static int DaysOfMonth(int year, int month) =>
        month switch
        {
            1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
            4 or 6 or 9 or 11 => 30,
            2 => year % 4 == 0 ? 29 : 28,
            _ => throw new ArgumentOutOfRangeException()
        };

    private static readonly Gen<int> SeqNo =
        Gen.Int[0, 9999];

    private static readonly Gen<PersonNummerData> GenPersonNummerData =
        GenYear.SelectMany(y =>
            GenMonth.SelectMany(m =>
                Gen.Int[1, DaysOfMonth(y, m)].SelectMany(d =>
                    SeqNo.Select(seq => new PersonNummerData(y, m, d, seq)))));

    private static readonly Gen<string> Hyphenation =
        Gen.OneOfConst("", "-");

    private static Gen<string> Render(PersonNummerData pd)
    {
        var y2 = pd.Year.ToString()[2..];
        var y4 = pd.Year.ToString();
        var m2 = pd.Month.ToString("D2");
        var d2 = pd.Day.ToString("D2");
        var s4 = pd.Num.ToString("D4");

        return Gen.OneOfConst(y2, y4)
            .SelectMany(y => Hyphenation.Select(h => $"{y}{m2}{d2}{h}{s4}"));
    }

    public static readonly Gen<TestPair> GenTestPair =
        GenPersonNummerData.SelectMany(pd =>
            Render(pd).Select(str => new TestPair(pd, str)));
}