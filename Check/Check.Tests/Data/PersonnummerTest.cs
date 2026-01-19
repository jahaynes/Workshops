using System;
using System.Linq;
using Check.Data;
using CsCheck;
using NUnit.Framework;

namespace Check.Tests.Data;

[TestFixture]
[TestOf(typeof(PersonKey))]
public class PersonnummerTest
{
    [Theory]
    public void Method()
    {
        PersonnummerTestData.GenTestPair.List.Sample(tps =>
        {
            foreach (var tp in tps)
            {
                Console.WriteLine(tp.Str);
                PersonKey.From(tp.Str);
            }
        });
    }
}

readonly record struct PersonNummerData(int Year, int Month, int Day, int Seq3, int LuhnDigit);

readonly record struct TestPair(PersonNummerData Data, string Str);

internal static class PersonnummerTestData
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

    private static readonly Gen<int> SeqNo3 =
        Gen.Int[0, 999];

    private static readonly Gen<PersonNummerData> GenPersonNummerData =
        GenYear.SelectMany(y =>
            GenMonth.SelectMany(m =>
                Gen.Int[1, DaysOfMonth(y, m)].SelectMany(d =>
                    SeqNo3.Select(seq3 =>
                        new PersonNummerData(y, m, d, seq3, LuhnDigit(y, m, d, seq3))))));

    private static int LuhnDigit(int y, int m, int d, int seq3)
    {
        var yy = y.ToString("D4").Substring(2, 2);
        var mm = m.ToString("D2");
        var dd = d.ToString("D2");
        var sss = seq3.ToString("D3");
        return LuhnCheckDigit3($"{yy}{mm}{dd}{sss}");
    }

    private static int LuhnCheckDigit3(string prefix)
    {
        if (prefix.Length != 9)
        {
            throw new ArgumentException("BAD LENGTH");
        }

        var sum = prefix
            .Select(d => int.Parse($"{d}"))
            .Zip([2, 1, 2, 1, 2, 1, 2, 1, 2])
            .Select(pair => AddDigits(pair.First * pair.Second))
            .Sum();

        return sum % 10 == 0
            ? 0
            : 10 - sum % 10;

        static int AddDigits(int num)
        {
            return num.ToString().Select(d => int.Parse($"{d}")).Sum();
        }
    }

    private static readonly Gen<string> Hyphenation =
        Gen.OneOfConst("", "-");

    private static Gen<string> Render(PersonNummerData pd)
    {
        var y2 = pd.Year.ToString("D4").Substring(2, 2);
        var y4 = pd.Year.ToString("D4");
        var m2 = pd.Month.ToString("D2");
        var d2 = pd.Day.ToString("D2");
        var s3 = pd.Seq3.ToString("D3");
        var l1 = pd.LuhnDigit;

        return Gen.OneOfConst(y2, y4)
            .SelectMany(y => Hyphenation.Select(h => $"{y}{m2}{d2}{h}{s3}{l1}"));
    }

    public static readonly Gen<TestPair> GenTestPair =
        GenPersonNummerData.SelectMany(pd =>
            Render(pd).Select(str => new TestPair(pd, str)));
}