using Newtonsoft.Json;

namespace pb8.Stockfighter.Model
{
    public class BidAsk
    {
        public int Price;
        [JsonProperty("Qty")]
        public int Quantity;
        public bool IsBuy;
    }
}