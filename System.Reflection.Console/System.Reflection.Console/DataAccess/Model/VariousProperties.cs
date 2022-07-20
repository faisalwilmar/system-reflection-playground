using Newtonsoft.Json;
using System.Collections.Generic;

namespace System.Reflection.Console.DataAccess.Model
{
    public class VariousProperties
    {
        [JsonProperty(PropertyName = "category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "categoryLevel")]
        public int CategoryLevel { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "consolidatedPropertyValue")]
        public string ConsolidatedPropertyValue { get; set; }

        //[Required]
        [JsonProperty(PropertyName = "currentDateTime")]
        public DateTime CurrentDateTime { get; set; }

        [JsonProperty(PropertyName = "categoryTags")]
        public List<Tag> CategoryTags { get; set; }

        public class Tag
        {
            [JsonProperty(PropertyName = "key")]
            public string Key { get; set; }

            [JsonProperty(PropertyName = "value")]
            public string Value { get; set; }
        }
    }
}
