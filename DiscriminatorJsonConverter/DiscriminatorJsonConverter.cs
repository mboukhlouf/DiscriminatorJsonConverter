using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MBoukhlouf.Json
{
    public class DiscriminatorJsonConverter<TBase, TDiscriminator> : JsonConverter<TBase> where TBase : class
    {
        private readonly string name;
        private readonly Dictionary<TDiscriminator, Type> values;

        public DiscriminatorJsonConverter(string name)
        {
            this.name = name;
            values = new Dictionary<TDiscriminator, Type>();
        }

        public void HasValue<TDerived>(TDiscriminator value) where TDerived : TBase
        {
            values.Add(value, typeof(TDerived));
        }

        public override TBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var reader2 = reader;
            int objectCounter = 0;
            TDiscriminator type = default;
            do
            {
                var tokenType = reader.TokenType;
                if (tokenType == JsonTokenType.StartObject)
                {
                    objectCounter++;
                }
                else if (tokenType == JsonTokenType.EndObject)
                {
                    objectCounter--;
                }

                if (tokenType == JsonTokenType.PropertyName)
                {
                    var property = reader.GetString();
                    if (property.Equals(name,
                        options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        type = JsonSerializer.Deserialize<TDiscriminator>(ref reader);
                    }
                }

                if (objectCounter > 0)
                    reader.Read();
            }
            while (objectCounter > 0);

            if (type == null)
            {
                throw new JsonException("PaymentType not specified.");
            }

            if (!values.ContainsKey(type))
            {
                throw new JsonException($"{type} value is not supported.");
            }

            return (TBase)JsonSerializer.Deserialize(ref reader2, values[type], options);
        }

        public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
