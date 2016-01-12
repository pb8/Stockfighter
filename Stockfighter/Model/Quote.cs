using System;

namespace pb8.Stockfighter.Model
{
    public class Quote: ResponseBase
    {
        public string Symbol;
        public string Venue;
        public int Bid;
        public int Ask;
        public int BidSize;
        public int AskSize;
        public int BidDepth;
        public int AskDepth;
        public int Last;
        public int LastSize;
        public DateTime LastTrade;
        public DateTime QuoteTime;
    }
}