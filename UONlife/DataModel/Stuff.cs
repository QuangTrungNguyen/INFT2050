using Newtonsoft.Json;

namespace UONlife.DataModel
{
    class Stuff
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "price")]
        public string price { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public string contact { get; set; }

        [JsonProperty(PropertyName = "imageaddress")]
        public string imageAddress { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public string publisher { get; set; }
    }
}
