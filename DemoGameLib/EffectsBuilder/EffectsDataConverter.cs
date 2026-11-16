
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DemoGameLib
{
    internal sealed class EffectDataConverter
        : JsonConverter<EffectData>
    {
        public override EffectData Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);

            JsonElement root = document.RootElement;


            if (!root.TryGetProperty("Type", out JsonElement typeElement))
            {
                throw new JsonException("Effect doesn't contain 'Type'.");
            }


            string typeName = typeElement.GetString() ?? 
                throw new JsonException("Effect Type is null.");


            if (!EffectsRegister.EFFECT_TYPES.TryGetValue(
                    typeName,
                    out EffectRegistration? registration))
            {
                throw new JsonException($"Unknown effect type: '{typeName}'.");
            }


            EffectData? result = JsonSerializer.Deserialize(
                root.GetRawText(),
                registration.DataType,
                options)
            as EffectData;


            return result ?? 
                throw new JsonException($"Cannot deserialize effect '{typeName}'.");
        }


        public override void Write(Utf8JsonWriter writer,
            EffectData value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(
                writer,
                value,
                value.GetType(),
                options);
        }
    }
}