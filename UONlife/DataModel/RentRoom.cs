using Newtonsoft.Json;

namespace UONlife.DataModel
{
    class RentRoom
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "roomtitle")]
        public string roomTitle { get; set; }

        [JsonProperty(PropertyName = "price")]
        public string price { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string address { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "bedrooms")]
        public string bedrooms { get; set; }

        [JsonProperty(PropertyName = "bathrooms")]
        public string bathrooms { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public string contact { get; set; }

        [JsonProperty(PropertyName = "altitude")]
        public double altitude { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double latitude { get; set; }

        [JsonProperty(PropertyName = "imageaddress")]
        public string imageAddress { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public string publisher { get; set; }

    }
}
