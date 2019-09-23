using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WM.Common.Serializers
{
    public class ApiPatchJsonSeializer : JsonConverter
    {
        private readonly Type[] _types;

        public ApiPatchJsonSeializer(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                writer.WriteStartArray();
                dynamic patch = Convert.ChangeType(value, _types[0]);

                foreach (var item in (patch.Operations))
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("value");
                    writer.WriteRawValue(JsonConvert.SerializeObject(item.Value));

                    writer.WritePropertyName("path");
                    writer.WriteValue(item.Path != null ? item.Path.ToString() : "");

                    writer.WritePropertyName("op");
                    writer.WriteValue(item.Op != null ? item.Op.ToString() : "");

                    writer.WritePropertyName("from");                      
                    writer.WriteValue(item.FromProperty != null ? item.FromProperty.ToString() : "");

                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }
    }
}
