using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetStash.Core
{
    [JsonObject]
    public class NetStashEvent
    {
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "exception-message")]
        public string ExceptionMessage { get; set; }
        
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

        /// <summary>
        /// Converte o objeto em dicionario
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"timestamp", Timestamp},
                {"message", Message},
                {"error.message", ExceptionMessage},
                {"error.details", ExceptionDetails},
                {"source", Source},
                {"level", Level},
                {"machine-name", Machine},
                {"index-name", Index}
            };

            if (Fields != null && Fields.Count > 0)
                foreach (var item in Fields)
                {
                    var key = item.Key;
                    var cont = 0;

                    while (dictionary.ContainsKey(item.Key))
                    {
                        cont++;
                        key = $"{key}_{cont}";
                    }

                    dictionary.Add(key, item.Value);
                }

            return dictionary;
        }
    }
}
