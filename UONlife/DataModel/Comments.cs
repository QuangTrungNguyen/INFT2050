using Newtonsoft.Json;

namespace UONlife.DataModel
{
    class Comments
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "from")]
        public string from { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string to { get; set; }

        [JsonProperty(PropertyName = "level")]
        public string level { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string comment { get; set; }
    }
}
