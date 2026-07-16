using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoverDotNet.Core.Coprocessor;

/// <summary>
/// Indicates whether the router should continue processing a client request, or
/// immediately terminate it with a given HTTP status code.
/// </summary>
/// <remarks>
/// Serializes to the string <c>"continue"</c>, or to <c>{ "break": statusCode }</c>,
/// matching the Apollo Router coprocessor <c>control</c> property.
/// </remarks>
[JsonConverter(typeof(CoprocessorControlJsonConverter))]
public sealed class CoprocessorControl
{
    /// <summary>The <c>"continue"</c> control value, allowing the router to proceed as usual.</summary>
    public static readonly CoprocessorControl Continue = new(isBreak: false, breakStatusCode: null);

    private CoprocessorControl(bool isBreak, int? breakStatusCode)
    {
        IsBreak = isBreak;
        BreakStatusCode = breakStatusCode;
    }

    /// <summary>Whether this control value terminates the client request.</summary>
    public bool IsBreak { get; }

    /// <summary>The HTTP status code to return to the client, when <see cref="IsBreak"/> is <see langword="true"/>.</summary>
    public int? BreakStatusCode { get; }

    /// <summary>Creates a control value that immediately terminates the client request.</summary>
    /// <param name="statusCode">The HTTP status code to return to the client.</param>
    public static CoprocessorControl Break(int statusCode) => new(isBreak: true, breakStatusCode: statusCode);
}

/// <summary>
/// Converts <see cref="CoprocessorControl"/> to/from the JSON shape used by the
/// Apollo Router coprocessor contract.
/// </summary>
internal sealed class CoprocessorControlJsonConverter : JsonConverter<CoprocessorControl>
{
    /// <inheritdoc/>
    public override CoprocessorControl Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return CoprocessorControl.Continue;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            int? breakStatusCode = null;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "break")
                {
                    reader.Read();
                    breakStatusCode = reader.GetInt32();
                }
            }

            return breakStatusCode.HasValue
                ? CoprocessorControl.Break(breakStatusCode.Value)
                : CoprocessorControl.Continue;
        }

        throw new JsonException("Unexpected token when parsing the coprocessor 'control' property.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, CoprocessorControl value, JsonSerializerOptions options)
    {
        if (value.IsBreak)
        {
            writer.WriteStartObject();
            writer.WriteNumber("break", value.BreakStatusCode!.Value);
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteStringValue("continue");
        }
    }
}
