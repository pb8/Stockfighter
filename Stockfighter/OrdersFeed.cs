using System;
using Newtonsoft.Json;
using pb8.Stockfighter.Model;
using WebSocket4Net;

namespace pb8.Stockfighter
{
    public class OrdersFeed
    {
        private readonly string _account;
        private readonly string _venue;
        private readonly string _symbol;      
        private readonly bool _reconnect;
        private WebSocket _ws;

        public OrdersFeed(string account, string venue, bool reconnect)
        {
            _account = account;
            _venue = venue;
            _reconnect = reconnect;
        }

        public OrdersFeed(string account, string venue, bool reconnect, string symbol)
            : this(account, venue, reconnect)
        {
            _symbol = symbol;
        }

        public EventHandler<Order> OrderReceived;
        public EventHandler Started;
        public EventHandler Stopped;
        public EventHandler<string> ErrorOccured;

        public void Start()
        {
            if (_ws != null) return;

            string wsUrl;
            if (!string.IsNullOrEmpty(_symbol))
            {
                wsUrl = string.Format("wss://www.stockfighter.io/ob/api/ws/{0}/venues/{1}/executions/stocks/{2}", _account, _venue, _symbol);
            }
            else
            {
                wsUrl = string.Format("wss://www.stockfighter.io/ob/api/ws/{0}/venues/{1}/executions", _account, _venue);
            }

            _ws = new WebSocket(wsUrl);
            _ws.MessageReceived += MessageReceived;
            _ws.Opened += (s, e) => { if (Started != null) Started(this, EventArgs.Empty); };
            _ws.Error += (s, e) => { if (ErrorOccured != null) ErrorOccured(this, e.Exception.Message); };
            _ws.Closed += (s, e) =>
            {
                if (Stopped != null) Stopped(this, EventArgs.Empty);
                if (_reconnect) _ws.Open();
            };            
            
            _ws.Open();

        }

        private void MessageReceived(object s, MessageReceivedEventArgs e)
        {
            OrderMessage message = null;

            try
            {
                message = JsonConvert.DeserializeObject<OrderMessage>(e.Message);
            }
            catch (Exception ex)
            {
                ErrorOccured(this, ex.Message);
            }

            if (message != null && message.Ok && message.Order != null && OrderReceived != null)
            {
                OrderReceived(this, message.Order);
            }
        }

        private void Stop()
        {
            if (_ws == null) return;

            _ws.Close();
            _ws = null;
        }

        public void Dispose()
        {
            Stop();
        }

        private class OrderMessage
        {
            public bool Ok;
            public Order Order;
        }
    }
}