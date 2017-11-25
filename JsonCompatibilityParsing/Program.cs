using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace JsonCompatibilityParsing
{
    class Program
    {
        static Data Expected = new Data
        {
            First = "Hello",
            Second = "World",
        };

        static string[] JsonBlobs => new[] {
            JArray.FromObject(new [] {
                "Hello",
                "World",
            }).ToString(),
            JObject.FromObject(new
            {
                StringValue = "Hello",
                SecondValue = "World"
            }).ToString(),
            JObject.FromObject(new
            {
                First = "Hello",
                Second = "World"
            }).ToString(),
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Expected");
            Console.WriteLine(JObject.FromObject(Expected).ToString());
            Console.WriteLine("");

            // Test building from scratch
            JsonBlobs
                .Select(s => new
                {
                    Json = s,
                    Object = JToken.Parse(s).ToObject<Data>()
                })
                .ToList()
                .ForEach(obj =>
                {
                    Console.WriteLine("Input (Creation):");
                    Console.WriteLine(obj.Json);
                    Console.WriteLine($"Got Expected: {obj.Object == Expected}\n");
                });
            Console.ReadKey();
        }
    }
}