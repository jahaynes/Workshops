using System.Text.Json.Nodes;
using CsCheck;

namespace Check.Tests.Data;

public class JsonTest
{
}

internal static class JsonTestData
{
    public static readonly Gen<string> GenString = Gen.String[Gen.Char.AlphaNumeric, 2, 5];

    public static readonly Gen<JsonNode> GenJsonValue = Gen.OneOf<JsonNode>(
        Gen.Bool.Select(x => JsonValue.Create(x)),
        Gen.Byte.Select(x => JsonValue.Create(x)),
        Gen.Char.AlphaNumeric.Select(x => JsonValue.Create(x)),
        Gen.DateTime.Select(x => JsonValue.Create(x)),
        Gen.DateTimeOffset.Select(x => JsonValue.Create(x)),
        Gen.Decimal.Select(x => JsonValue.Create(x)),
        Gen.Double.Select(x => JsonValue.Create(x)),
        Gen.Float.Select(x => JsonValue.Create(x)),
        Gen.Guid.Select(x => JsonValue.Create(x)),
        Gen.Int.Select(x => JsonValue.Create(x)),
        Gen.Long.Select(x => JsonValue.Create(x)),
        Gen.SByte.Select(x => JsonValue.Create(x)),
        Gen.Short.Select(x => JsonValue.Create(x)),
        GenString.Select(x => JsonValue.Create(x)),
        Gen.UInt.Select(x => JsonValue.Create(x)),
        Gen.ULong.Select(x => JsonValue.Create(x)),
        Gen.UShort.Select(x => JsonValue.Create(x)));

    public static readonly Gen<JsonNode> GenJsonNode = Gen.Recursive<JsonNode>((depth, genJsonNode) =>
    {
        if (depth == 5) return GenJsonValue;
        var genJsonObject = GenString.Dictionary(genJsonNode.Null())[0, 5].Select(d => new JsonObject(d));
        var genJsonArray = genJsonNode.Null().Array[0, 5].Select(i => new JsonArray(i));
        return Gen.OneOf(genJsonObject, genJsonArray, GenJsonValue);
    });
}