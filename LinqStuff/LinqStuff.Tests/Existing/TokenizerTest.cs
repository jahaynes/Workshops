using System;
using System.Collections.Immutable;
using JetBrains.Annotations;
using LinqStuff.Either;
using LinqStuff.Existing;
using Xunit;
using Shouldly;

namespace LinqStuff.Tests.Existing;

[TestSubject(typeof(Tokenizer))]
public class TokenizerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TokenizerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]

    // Good
    [InlineData("test", new[] { "test" })]
    [InlineData("multiple   spaces", new[] { "multiple", "spaces" })]
    [InlineData("\"quotes\"", new[] { "quotes" })]
    [InlineData("\"quotes and spaces\"", new[] { "quotes and spaces" })]
    [InlineData("\"quotes with space \"", new[] { "quotes with space " })]
    [InlineData("\"quotes with trailing space\" ", new[] { "quotes with trailing space" })]
    [InlineData("\"quotes \\\" blabla", new[] { "quotes \\\" blabla" })]
    [InlineData("\"\"dbl \"quotes\"\"\"", new[] { "dbl \"quotes\"" })]

    // Bad
    [InlineData("\"quotes \"blabla", null, true)] // Must have space after quotes before next token
    public void TestTokenize(string input, string[] tokens, bool throws = false)
    {
        var expectedTokens = tokens;
        if (throws)
        {
            Should.Throw<ArgumentException>(() => Tokenizer.Tokenize(input), $"input: {input}");
        }
        else
        {
            var actualTokens = Tokenizer.Tokenize(input);
            actualTokens.ShouldBe(expectedTokens);
        }
    }


    [Theory]

    // Good
    [InlineData("test", new[] { "test" })]
    [InlineData("multiple   spaces", new[] { "multiple", "spaces" })]
    [InlineData("\"quotes\"", new[] { "quotes" })]
    [InlineData("\"quotes and spaces\"", new[] { "quotes and spaces" })]
    [InlineData("\"quotes with space \"", new[] { "quotes with space " })]
    [InlineData("\"quotes with trailing space\" ", new[] { "quotes with trailing space" })]
    [InlineData("\"quotes \\\" blabla", new[] { "quotes \\\" blabla" })]
    [InlineData("\"\"dbl \"quotes\"\"\"", new[] { "dbl \"quotes\"" })]

    // Bad
    [InlineData("\"quotes \"blabla", null, true)] // Must have space after quotes before next token
    public void TestTokenize2(string input, string[] tokens, bool throws = false)
    {
        switch (ReplacementTokenizer.CommandLine().Run(input))
        {
            case Left<string, Tuple<ImmutableList<string>, string>> left:
                _testOutputHelper.WriteLine("{" + input + "}");
                _testOutputHelper.WriteLine("Fail: " + left.L);
                throw new Exception("Test failed");

            case Right<string, Tuple<ImmutableList<string>, string>> right:
                _testOutputHelper.WriteLine("{" + input + "}");
                _testOutputHelper.WriteLine("Success, Leftover: {" + right.R.Item2 + "}");
                foreach (var se in right.R.Item1)
                {
                    _testOutputHelper.WriteLine("{" + se + "}");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}