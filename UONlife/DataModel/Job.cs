using Newtonsoft.Json;

namespace UONlife.DataModel
{
    class Job
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }

        // Type: full time, part time or contract
        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "classification")]
        public string classification { get; set; }

        [JsonProperty(PropertyName = "salary")]
        public string salary { get; set; }

        [JsonProperty(PropertyName = "workingPlace")]
        public string workingPlace { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }

        [JsonProperty(PropertyName = "requirements")]
        public string requirements { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public string contact { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public string publisher { get; set; }

    }
}
