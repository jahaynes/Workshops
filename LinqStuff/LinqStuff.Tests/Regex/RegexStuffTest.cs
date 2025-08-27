using System.Text.RegularExpressions;
using Xunit;

namespace LinqStuff.Tests.Regex;

public class RegexStuffTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RegexStuffTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void METHOD()
    {
        var input = "johndoe@example.com";
        var regex = new System.Text.RegularExpressions.Regex(@"[^@]+@");
        Match match = regex.Match(input);
        _testOutputHelper.WriteLine(match.ToString());
    }
}