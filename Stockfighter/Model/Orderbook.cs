using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace pb8.Stockfighter.Model
{
    public class Orderbook : ResponseBase
    {
        public string Venue;
        public string Symbol;
        public List<BidAsk> Bids;
        public List<BidAsk> Asks;
        [JsonProperty("ts")]
        public DateTime Timestamp;
    }
}