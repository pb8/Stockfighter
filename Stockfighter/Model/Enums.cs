using System.Runtime.Serialization;

namespace pb8.Stockfighter.Model
{
    public class Enums
    {
        public enum Direction
        {
            [EnumMember(Value = "buy")]
            Buy,
            [EnumMember(Value = "sell")]
            Sell
        }

        public enum OrderType
        {
            [EnumMember(Value = "limit")]
            Limit,
            [EnumMember(Value = "market")]
            Market,
            [EnumMember(Value = "fill-or-kill")]
            FillOrKill,
            [EnumMember(Value = "immediate-or-cancel")]
            ImmediateOrCancel
        }
    }
}
