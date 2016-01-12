using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pb8.Stockfighter.Model
{
    public class OrderRequest
    {
        public string Account;
        public string Venue;
        [JsonProperty("Stock")]
        public string Symbol;
        public int Price;
        [JsonProperty("Qty")]
        public int Quantity;
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.Direction Direction;
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.OrderType OrderType;
    }
}