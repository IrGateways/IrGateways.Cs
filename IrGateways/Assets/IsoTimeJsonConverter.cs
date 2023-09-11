using System.Text.Json;
using System.Text.Json.Serialization;

namespace IrGateways.Assets;

public class IsoTimeJsonConverter : JsonConverter<DateTime>
{
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }
}