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
            var allConstructors = objectType.GetConstructors();
            var constructors = allConstructors
                .Where(c => c.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(JsonConstructorAttribute)) != null)
                .ToArray();
            if (constructors.Length > 1) throw new Exception("Ambiguous JsonConstructorAttribute");
            if (constructors.Length == 0)
            {
                constructors = allConstructors
                    .Where(c =>
                    {
                        var args = c.GetParameters();
                        if (args.Length < 1) return false;
                        if (args.Length > 2) return false;
                        if (!typeof(JToken).IsAssignableFrom(args[0].ParameterType)) return false;
                        if (args.Length >= 2)
                        {
                            if (args[1].ParameterType != typeof(object)) return false;
                        }
                        return true;
                    })
                    .ToArray();
            }
            if (constructors.Length > 1) throw new Exception("Ambiguous JToken constructor");
            if (constructors.Length == 0) throw new Exception($"No JToken constructor found {objectType.Name}(JToken token[, object existingValue = null])");
            var constructor = constructors[0];
            var parameters = constructor.GetParameters().ToArray();
            var token = JToken.ReadFrom(reader).ToObject(parameters[0].ParameterType);
            if (parameters.Length == 2)
            {
                return constructor.Invoke(new object[] { token, existingValue });
            }
            return constructor.Invoke(new object[] { token });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // This should never be called because CanWrite returns false
            throw new NotImplementedException();
        }
    }
}
