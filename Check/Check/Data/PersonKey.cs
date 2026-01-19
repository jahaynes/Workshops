using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Check.Data;

public class InvalidPersonKeyFormatException(string msg) : Exception(msg);

public sealed record PersonKey
{
    public string Value { get; }

    private PersonKey(string value)
    {
        Value = value;
    }

    public static PersonKey From(string raw)
    {
        var normalized = Normalize(raw);

        if (!IsValidSwedishPersonalNumber(normalized))
        {
            throw new InvalidPersonKeyFormatException($"Invalid personal identity number: {raw}");
        }

        return new PersonKey(normalized);
    }


    private static readonly char[] UnicodeHyphens =
    [
        '\u2010', '\u2011', '\u2012', '\u2013', '\u2014', '\u2015', '\u2212'
    ];

    private static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new InvalidPersonKeyFormatException("Input cannot be null or whitespace.");
        }

        string normalized = input.Trim();

        foreach (var hyphen in UnicodeHyphens)
        {
            normalized = normalized.Replace(hyphen, '-');
        }

        // Remove "personnummer" prefixes
        normalized = Regex.Replace(normalized, "(?i)person(nr|nummer)\\s*[:\\-\\s]*", "");

        // Remove whitespace, slashes, dots
        normalized = Regex.Replace(normalized, "[\\s./]", "");

        // Only digits, +, -
        normalized = Regex.Replace(normalized, "[^0-9+\\-]", "");

        bool plusUsed = normalized.Contains("+");
        if (plusUsed)
        {
            normalized = normalized.Replace("+", "-");
        }

        // Keep only the last hyphen
        if (normalized.Count(c => c == '-') > 1)
        {
            var parts = normalized.Split('-');
            normalized = string.Join("", parts[..^1]) + "-" + parts[^1];
        }

        // Combine into digits
        string digits = normalized.Contains('-')
            ? string.Concat(normalized.Split('-', 2))
            : normalized;

        // Convert 10-digit numbers to 12 digits
        if (digits.Length == 10)
        {
            digits = To12Digits(digits, plusUsed);
        }

        if (digits.Length != 12)
        {
            throw new InvalidPersonKeyFormatException("Personal number must be 10 or 12 digits.");
        }

        return digits;
    }

    private static string To12Digits(string digits10, bool plusUsed)
    {
        string yy = digits10.Substring(0, 2);
        string rest = digits10.Substring(2);

        int year = int.Parse(yy);
        int currentYY = DateTime.Now.Year % 100;

        int century = plusUsed
            ? ((DateTime.Now.Year / 100) - 1) * 100 // 100+ years old
            : (year > currentYY ? 1900 : 2000);

        return $"{century + year}{rest}";
    }


    // SWEDISH PERSONAL NUMBER VALIDATION (Luhn)

    private static bool IsValidSwedishPersonalNumber(string digits12)
    {
        // must be 12 digits before Luhn check
        if (!Regex.IsMatch(digits12, @"^\d{12}$"))
        {
            throw new ArgumentException("JOHN REGEX");
        }

        string digits10 = digits12.Substring(2); // Luhn uses YYMMDDNNNC

        return LuhnCheck(digits10);
    }

    private static bool LuhnCheck(string num)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = num.Length - 1; i >= 0; i--)
        {
            int n = num[i] - '0';

            if (alternate)
            {
                n *= 2;
                if (n > 9)
                {
                    n -= 9;
                }
            }

            sum += n;
            alternate = !alternate;
        }
        
        if (sum % 10 == 0)
        {
            return true;
        }
        else
        {
            throw new ArgumentException($"BAD SUM: {sum % 10}");
        }
    }
}