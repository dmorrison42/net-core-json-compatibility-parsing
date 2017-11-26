using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace JsonCompatibilityParsing
{
    [JsonConverter(typeof(JsonTokenConstructor))]
    class Data
    {
        public string First { set; get; }
        public string Second { set; get; }

        public Data() { }

        public Data(JToken token)
        {
            var fail = new Exception("This just isn't what I'm looking for in an object!");
            switch (token)
            {
                case JArray array:
                    if (array.Count != 2) throw fail;
                    First = array[0].Value<string>();
                    Second = array[1].Value<string>();
                    return;
                case JObject obj:
                    First = obj.Value<string>("First") ?? obj.Value<string>("StringValue") ?? obj.Value<string>("String");
                    Second = obj.Value<string>("Second") ?? obj.Value<string>("SecondValue");
                    return;
            }
            throw fail;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (Data)obj;
            return First == other.First && Second == other.Second;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public static bool operator ==(Data current, object other)
        {
            return current.Equals(other);
        }

        public static bool operator !=(Data current, object other)
        {
            return !(current == other);
        }

        public override string ToString()
        {
            return $"Data: ({First}, {Second})";
        }
    }
}
