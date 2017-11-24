# .net Core JSON Compatibility Parsing

This is just an example of how to use Newtonsoft.Json ([Json.NET](https://json.net)) to be able to parse different JSON objects to a single equivalent object.

## Use Case

### I changed a property name

Yes, this is a breaking change, but it's sometimes nice to be able to read save files from past versions of applications.

### I really didn't want to store this as an object

Sometimes (albeit rarely) it makes sense to store something as an object, but it still makes sense to store it as an array, and it would be way more space efficient (say a point on a graph).

This isn't even necessarily a breaking change depending on how much your users are dependent on the JSON output (for instance save files vs APIs).

### I Completely changed this data's layout

I don't have an example of this, but because you receive a JObject, it's not really hard to do anything you want to do to read data in.

## How it works

There's not a lot of code here, so reading it all isn't hard, but heres the tl;dr:

- Add a `JsonConverterAttribute` to your class ([Data.cs](JsonCompatibilityParsing/Data.cs))

    ```C#
    [JsonConverter(typeof(DataJsonConverter))]
    ```

- Add a JSON Converter ([DataJsonConverter.cs](JsonCompatibilityParsing/DataJsonConverter.cs))
    - Implement the `JsonConverter` abstract class
    - Turn off writing if your class is already doing it correctly
        ```C#
        public override bool CanWrite => false;
        ```
    - Convert your `JsonReader` to a `JToken` (optional)
        ```C#
        var token = JToken.ReadFrom(reader);
        ```
    - ...
    - Profit!
