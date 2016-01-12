using System.Collections.Generic;

namespace pb8.Stockfighter.Model
{
    public class OrderStatuses : ResponseBase
    {
        public string Venue;
        public List<Order> Orders;
    }
}