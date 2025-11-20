#nullable enable

using System;
using LinqStuff.Existing;
using Xunit;

namespace LinqStuff.Tests.Existing;

public class Bleh
{
    [Fact]
    void Test()
    {
        Tokenizer.Tokenize("one \"two\" three");
        Tokenizer.Tokenize("");
    }

    static void Foo(Person p)
    {
        Console.WriteLine(p.Name.ToUpper());
    }
}

class Person
{
    public required string? Name { get; set; }
}