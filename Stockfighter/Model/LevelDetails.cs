using System.Collections.Generic;
using Newtonsoft.Json;

namespace pb8.Stockfighter.Model
{
    public class LevelDetails : ResponseBase
    {
        public string Account;
        public string InstanceId;        
        public LevelInstructions Instructions;
        public int SecondsPerTradingDay;
        [JsonProperty("tickers")]
        public List<string> Symbols;
        public List<string> Venues;
        //TODO balances


        public class LevelInstructions
        {
            public string Instructions;
            [JsonProperty("order types")]
            public string OrderTypes;
        }
    }
}