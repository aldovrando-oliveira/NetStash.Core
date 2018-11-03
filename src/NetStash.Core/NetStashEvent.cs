using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetStash
{
    [JsonObject]
    public class NetStashEvent
    {
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "exception-details")]
        public string ExceptionDetails { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "level")]
        public string Level { get; set; }

        [JsonProperty(PropertyName = "machine-name")]
        public string Machine { get; set; }

        [JsonProperty(PropertyName = "index-name")]
        public string Index { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public Dictionary<string, string> Fields { get; set; }

        public NetStashEvent()
        {
            Timestamp = DateTime.Now;
        }

        public IDictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("timestamp", Timestamp);
            dictionary.Add("message", Message);
            dictionary.Add("error", ExceptionDetails);
            dictionary.Add("source", Source);
            dictionary.Add("level", Level);
            dictionary.Add("machine-name", Machine);
            dictionary.Add("index-name", Index);

            if (Fields != null)
                foreach(var item in Fields)
                    dictionary.Add(item.Key, item.Value);

            return dictionary;
        }
    }
}
