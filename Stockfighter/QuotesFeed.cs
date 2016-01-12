using System;
using Newtonsoft.Json;
using pb8.Stockfighter.Model;
using WebSocket4Net;

namespace pb8.Stockfighter
{
    public class QuotesFeed : IDisposable
    {
        private readonly string _account;
        private readonly string _venue;
        private readonly string _symbol;      
        private readonly bool _reconnect;
        private WebSocket _ws;

        public QuotesFeed(string account, string venue, bool reconnect)
        {
            _account = account;
            _venue = venue;
            _reconnect = reconnect;
        }

        public QuotesFeed(string account, string venue, bool reconnect, string symbol)
            : this(account, venue, reconnect)
        {
            _symbol = symbol;
        }

        public EventHandler<Quote> QuoteReceived;
        public EventHandler Started;
        public EventHandler Stopped;
        public EventHandler<string> ErrorOccured;

        public void Start()
        {
            if (_ws != null) return;

            string wsUrl;
            if (!string.IsNullOrEmpty(_symbol))
            {
                wsUrl = string.Format("wss://www.stockfighter.io/ob/api/ws/{0}/venues/{1}/tickertape/stocks/{2}", _account, _venue, _symbol);
            }
            else
            {
                wsUrl = string.Format("wss://www.stockfighter.io/ob/api/ws/{0}/venues/{1}/tickertape", _account, _venue);
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
            QuoteMessage message = null;

            try
            {
                message = JsonConvert.DeserializeObject<QuoteMessage>(e.Message);
            }
            catch (Exception ex)
            {
                ErrorOccured(this, ex.Message);
            }

            if (message != null && message.Ok && message.Quote != null && QuoteReceived != null)
            {
                QuoteReceived(this, message.Quote);
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

        private class QuoteMessage
        {
            public bool Ok;
            public Quote Quote;
        }
    }
}
