using Newtonsoft.Json;


namespace UONlife.DataModel
{
    class Textbook
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "bookname")]
        public string bookName { get; set; }

        [JsonProperty(PropertyName = "courseid")]
        public string courseID { get; set; }

        [JsonProperty(PropertyName = "depreciation")]
        public string depreciation { get; set; }

        [JsonProperty(PropertyName = "price")]
        public string price { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }

        [JsonProperty(PropertyName = "contact")]
        public string contact { get; set; }


        // Image
        [JsonProperty(PropertyName = "imageaddress")]
        public string imageAddress { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public string publisher { get; set; }

        public override string ToString()
        {
            return this.bookName;
        }

    }
}
