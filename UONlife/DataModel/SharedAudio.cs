using Newtonsoft.Json;

namespace UONlife.DataModel
{
    class SharedAudio
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "provider")]
        public string provider { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string fileName { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string uri { get; set; }

    }
}
