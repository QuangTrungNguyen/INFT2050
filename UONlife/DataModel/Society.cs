using Newtonsoft.Json;

namespace UONlife.DataModel
{
    class Society
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "societyname")]
        public string societyName { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }

        [JsonProperty(PropertyName = "places")]
        public string places { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public string contact { get; set; }

        [JsonProperty(PropertyName = "clubwebsite")]
        public string clubwebsite { get; set; }

        [JsonProperty(PropertyName = "imageaddress")]
        public string imageAddress { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public string publisher { get; set; }
    }
}
