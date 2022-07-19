using Newtonsoft.Json;

namespace System.Reflection.Console.Dto
{
    public class KeyValueObject
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
    }
}
