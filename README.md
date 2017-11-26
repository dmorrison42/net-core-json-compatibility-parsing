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

### Usage

There's not a lot of code here, so reading it all isn't hard, but heres the tl;dr:

- Add a `JsonTokenConstructorAttribute` to your class ([Data.cs](JsonCompatibilityParsing/Data.cs))
    ```C#
    [JsonConverter(typeof(JsonTokenConstructor))]
    ```
- Create a constructor who's first argument inherits from JToken (second argument is optional)
    ```C#
    Data(JObject obj, object existingValue)
    ```
- (Optional) Mark the constructor with the `JsonConstructorAttribute`
    ```C#
    [JsonConstructor]
    public Data(JToken token)
    ```
- ...
- Profit!

### How it works

- Implements the `JsonConverter` abstract class
- Turns off writing (your class should do that correctly)
    ```C#
    public override bool CanWrite => false;
    ```
- Converts the `JsonReader` to a `JToken`
    ```C#
    var token = JToken.ReadFrom(reader);
    ```
- Selects a constructor
    - Get all constructors
    - Filter out constructors with more than 2 parameters
    - Select only the constructors where the first parameter implements JToken
    - Filter out constructors where the second argument isn't object
    - If there is more than one constructor remaining
        - Get only the constructors that are the correct type (JObject or JArray)
        - If there are no constructors select only the JToken type
    - If there is exactly one remaining constructor, use that
- Pass the token and the existingValue (optional) to the selected constructor

### TODO:

- Learn what existingValue is for
- Handle Population
