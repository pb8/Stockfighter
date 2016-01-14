using Newtonsoft.Json;

namespace pb8.Stockfighter.Model
{
    public class LevelStatus : ResponseBase
    {
        public LevelStatusDetails Details;
        public bool Done;
        [JsonProperty("id")]
        public string InstanceId;
        public string State;

        public class LevelStatusDetails
        {
            public int EndOfTheWorldDay;
            public int TradingDay;
        }
    }
}