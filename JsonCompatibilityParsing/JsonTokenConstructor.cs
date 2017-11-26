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
            var token = JToken.ReadFrom(reader);
            var allConstructors = objectType.GetConstructors();
            var constructors = allConstructors
                .Where(c => c.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(JsonConstructorAttribute)) != null)
                .ToArray();
            if (constructors.Length == 0) constructors = allConstructors;

            var constructorInfo = constructors
                .Select(c => new
                {
                    Constructor = c,
                    Parameters = c.GetParameters().ToArray(),
                })
                // Ignore constructors with more than two parameters
                .Where(ci => ci.Parameters.Length <= 2)
                // Ignore constructors where the first argument isn't a JToken
                .Where(ci => typeof(JToken).IsAssignableFrom(ci.Parameters.FirstOrDefault()?.ParameterType))
                // Ignore constructors where the second argument isn't an object
                .Where(ci => new[] { null, typeof(object) }.Contains(ci.Parameters.Skip(1).FirstOrDefault()?.ParameterType))
                .ToArray();

            // Pick the most specific constructor from JArray, JObject, and JToken
            if (constructorInfo.Length > 1)
            {
                var filteredConstructorInfo = constructorInfo;
                switch (token.Type)
                {
                    case JTokenType.Array:
                        filteredConstructorInfo = constructorInfo
                            .Where(ci => ci.Parameters.First().ParameterType == typeof(JArray))
                            .ToArray();
                        break;
                    case JTokenType.Object:
                        filteredConstructorInfo = constructorInfo
                            .Where(ci => ci.Parameters.First().ParameterType == typeof(JObject))
                            .ToArray();
                        break;
                }
                if (filteredConstructorInfo.Length == 0)
                {
                    filteredConstructorInfo = constructorInfo
                        .Where(ci => ci.Parameters.First().ParameterType == typeof(JToken))
                        .ToArray();
                }
                if (filteredConstructorInfo.Length > 1) throw new Exception("Ambiguous JToken constructor");
                constructorInfo = filteredConstructorInfo;
            }
            if (constructorInfo.Length == 0) {
                throw new Exception($"No JToken constructor found {objectType.Name}(JToken token[, object existingValue = null])");
            }
            var constructor = constructorInfo[0].Constructor;
            var parameters = constructorInfo[0].Parameters;
            var castToken = token.ToObject(parameters[0].ParameterType);
            if (parameters.Length == 2)
            {
                return constructor.Invoke(new object[] { castToken, existingValue });
            }
            return constructor.Invoke(new object[] { castToken });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // This should never be called because CanWrite returns false
            throw new NotImplementedException();
        }
    }
}
