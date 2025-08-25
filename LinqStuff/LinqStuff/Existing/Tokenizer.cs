namespace LinqStuff.Existing;

public static class Tokenizer
{
    public static IEnumerable<string> Tokenize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Array.Empty<string>();
        }

        var tokens = new List<string>();
        var start = -1;
        var insideQuotes = false;
        var insideDoubleQuotes = false;
        char lastChar = '\0';

        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];

            if (currentChar == '"' && lastChar != '\\')
            {
                // Either entering or exiting double quotes
                if ((insideQuotes || insideDoubleQuotes) && lastChar == '"')
                {
                    if (!insideDoubleQuotes)
                    {
                        insideDoubleQuotes = true;
                        start = i;
                    }
                    else
                    {
                        // Let's eat all the quotes
                        while (i + 1 < input.Length && input[i + 1] == '"')
                        {
                            i++;
                        }

                        tokens.Add(input.Substring(start + 1, i - start - 2));
                        start = -1;
                        insideQuotes = false;
                        insideDoubleQuotes = false;
                        if (i + 1 < input.Length && !char.IsWhiteSpace(input[i + 1]))
                        {
                            throw new ArgumentException(
                                $"Invalid input: After a token surrounded by quotes there must be whitespace before next token! @{i}: {GetExceptionDetails(input, i)}");
                        }
                    }
                }
                else if (insideDoubleQuotes)
                {
                    // Do nothing
                }
                else
                {
                    // Toggle the insideQuotes flag
                    insideQuotes = !insideQuotes;

                    if (!insideQuotes && start != -1)
                    {
                        // End of quoted token
                        tokens.Add(input.Substring(start + 1, i - start - 1));
                        start = -1;
                        if (i + 1 < input.Length && !char.IsWhiteSpace(input[i + 1]))
                        {
                            throw new ArgumentException(
                                $"Invalid input: After a token surrounded by quotes there must be whitespace before next token! @{i}: {GetExceptionDetails(input, i)}");
                        }
                    }
                    else if (insideQuotes)
                    {
                        // Start of quoted token
                        start = i;
                    }
                }
            }
            else if (char.IsWhiteSpace(currentChar) && !insideQuotes)
            {
                if (start != -1)
                {
                    // End of a token
                    tokens.Add(input.Substring(start, i - start));
                    start = -1;
                }
            }
            else
            {
                if (start == -1)
                {
                    // Start of a new token
                    start = i;
                }
            }

            lastChar = currentChar;
        }

        // Add the last token if it exists
        if (start != -1)
        {
            if (insideQuotes || insideDoubleQuotes)
            {
                // Handle unclosed quotes by trimming the starting quote
                tokens.Add(input.Substring(start + 1, input.Length - start - 1));
            }
            else
            {
                tokens.Add(input.Substring(start, input.Length - start));
            }
        }

        return tokens.ToArray();

        static string GetExceptionDetails(string input, int i)
        {
            var left = Math.Max(0, i - 2);
            var right = Math.Min(input.Length - 1, i + 2);
            var result = ".." + input.Substring(left, i - left) + ">" + input[i] + "<" +
                         input.Substring(i + 1, right - i) + "..";
            return result;
        }
    }
}