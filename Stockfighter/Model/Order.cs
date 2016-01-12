using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pb8.Stockfighter.Model
{
    public class Order: ResponseBase
    {
        public string Symbol;
        public string Venue;
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.Direction Direction;
        [JsonProperty("OriginalQty")]
        public int OriginalQuantity;
        [JsonProperty("Qty")]
        public int Quantity;
        public int Price;
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.OrderType OrderType;
        public int Id;
        public string Account;
        [JsonProperty("ts")]
        public DateTime Timestamp;
        public List<Fill> Fills;
        public int TotalFilled;
        public bool Open;
    }
}