using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bulud.Base.Converters;

public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Accept any valid datetime format and parse as UTC
        var value = reader.GetString();
        return DateTime.Parse(value!).ToUniversalTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Always write as UTC ISO 8601 string with 'Z'
        writer.WriteStringValue(value.ToUniversalTime().ToString("o"));
    }
}
