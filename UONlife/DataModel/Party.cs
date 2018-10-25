using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UONlife.DataModel
{
    class Party
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "partytitle")]
        public string partyTitle { get; set; }

        [JsonProperty(PropertyName = "price")]
        public string price { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string address { get; set; }

        [JsonProperty(PropertyName = "crowd")]
        public string crowd { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public string contact { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string content { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string time { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public string publisher { get; set; }

        [JsonProperty(PropertyName = "imageaddress")]
        public string imageAddress { get; set; }
    }
}
