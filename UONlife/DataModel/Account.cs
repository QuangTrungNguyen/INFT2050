using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UONlife.DataModel
{
    public class Account
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set;}

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "storagespace")]
        public int StorageSpace { get; set; }
    }
}
