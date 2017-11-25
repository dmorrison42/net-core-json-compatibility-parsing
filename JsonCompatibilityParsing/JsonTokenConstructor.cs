using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace JsonCompatibilityParsing
{
    class JsonTokenConstructor : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var constructors = objectType.GetConstructors()
                .Where(c => c.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(JsonConstructorAttribute)) != null)
                .ToArray();
            if (constructors.Length != 1) throw new Exception("Must have exactly one JsonConstructor");
            var constructor = constructors[0];
            var token = JToken.ReadFrom(reader);
            return constructor.Invoke(new object[] { token });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // This should never be called because CanWrite returns false
            throw new NotImplementedException();
        }
    }
}
