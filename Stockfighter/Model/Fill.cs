using System;
using Newtonsoft.Json;

namespace pb8.Stockfighter.Model
{
    public class Fill
    {
        public int Price;
        [JsonProperty("qty")]
        public int Quantity;
        [JsonProperty("ts")]
        public DateTime Timestamp;
    }
}