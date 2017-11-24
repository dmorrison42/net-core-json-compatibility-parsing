using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonCompatibilityParsing
{
    class DataJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var fail = new Exception("This just isn't what I'm looking for in an object!");
            var token = JToken.ReadFrom(reader);
            switch (token)
            {
                case JArray array:
                    if (array.Count != 2) throw fail;
                    return new Data(array[0].Value<string>(), array[1].Value<string>());
                case JObject obj:
                    var stringValue = obj.Value<string>("StringValue") ?? obj.Value<string>("String");
                    var privateValue = obj.Value<string>("PrivateValue") ?? obj.Value<string>("Private");
                    return new Data(stringValue, privateValue);
                default:
                    throw fail;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // This should never be called because CanWrite returns false
            throw new NotImplementedException();
        }
    }
}
