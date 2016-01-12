using System.Collections.Generic;
using Newtonsoft.Json;

namespace pb8.Stockfighter.Model
{
    public class StocksOnVenue : ResponseBase
    {
        [JsonProperty("symbols")]
        public List<Stock> Stocks;
    }
}