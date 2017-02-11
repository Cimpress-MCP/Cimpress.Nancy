using Newtonsoft.Json;

namespace Cimpress.Nancy.Logging
{
    [JsonObject]
    public class JsonLog
    {
        [JsonProperty]
        public string Environment { get; set; }

        [JsonProperty]
        public string Host { get; set; }

        [JsonProperty("@timestamp")] //Elasticsearch special field
        public string TimeStamp { get; set; }

        [JsonProperty]
        public object Data { get; set; }

        [JsonProperty]
        public int MessageHash { get; set; }

        [JsonProperty]
        public string RenderedMessage { get; set; }

        [JsonProperty]
        public object ExceptionData { get; set; }

        [JsonProperty]
        public string Level { get; set; }

        [JsonProperty]
        public string Executable { get; set; }
    }
}
