using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using pb8.Stockfighter.Model;

namespace pb8.Stockfighter
{
    public class Api
    {
        private readonly Uri _apiBaseUri;
        private readonly string _apiWsUrl;
        private readonly string _apiKey;

        public Api(string apiKey)
        {
            _apiKey = apiKey;
            _apiBaseUri = new Uri("https://api.stockfighter.io/ob/api/");
            _apiWsUrl = "wss://www.stockfighter.io/ob/api/ws/";
        }

        public Api(string apiKey, string apiBaseUrl, string apiWsUrl)
        {
            _apiKey = apiKey;
            _apiBaseUri = new Uri(apiBaseUrl);
            _apiWsUrl = apiWsUrl;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = _apiBaseUri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Starfighter-Authorization", _apiKey);
            return client;
        }

        private async Task<T> ProcessHttpResponse<T>(HttpResponseMessage response) where T : ResponseBase
        {
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            else
            {
                var result = Activator.CreateInstance<T>();
                result.Ok = false;
                result.Error = string.Format("Unable to connect: {0} {1}", response.StatusCode, response.ReasonPhrase);
                return result;
            }
        }

        private async Task<T> HttpGet<T>(string requestUri) where T : ResponseBase
        {
            using (var client = GetHttpClient())
            {
                try
                {
                    var response = await client.GetAsync(requestUri).ConfigureAwait(false);
                    return await ProcessHttpResponse<T>(response).ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    var result = Activator.CreateInstance<T>();
                    result.Ok = false;
                    result.Error = string.Format("Error: {0}", ex.Message);
                    return result;
                }
            }
        }

        public async Task<ApiHeartbeat> GetApiHeartbeat()
        {
            return await HttpGet<ApiHeartbeat>("heartbeat");
        }

        public async Task<VenueHeartbeat> GetVenueHeartbeat(string venue)
        {
            return await HttpGet<VenueHeartbeat>(string.Format("venues/{0}/heartbeat", venue));
        }

        public async Task<StocksOnVenue> GetStocksOnVenue(string venue)
        {
            return await HttpGet<StocksOnVenue>(string.Format("venues/{0}/stocks", venue));
        }

        public async Task<Orderbook> GetOrderbook(string venue, string symbol)
        {
            return await HttpGet<Orderbook>(string.Format("venues/{0}/stocks/{1}", venue, symbol));
        }

        public async Task<Order> CreateOrder(OrderRequest request)
        {            
            using (var api = GetHttpClient())
            {
                try
                {
                    var orderUrl = string.Format("venues/{0}/stocks/{1}/orders", request.Venue, request.Symbol);
                    var orderJson = JsonConvert.SerializeObject(request);
                    var response = await api.PostAsync(orderUrl, new StringContent(orderJson)).ConfigureAwait(false);
                    return await ProcessHttpResponse<Order>(response).ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    var order = new Order {Ok = false, Error = string.Format("Error: {0}", ex.Message)};
                    return order;
                }                
            }
        }

        public async Task<Quote> GetQuote(string venue, string symbol)
        {
            return await HttpGet<Quote>(string.Format("venues/{0}/stocks/{1}/quote", venue, symbol));
        }

        public async Task<Order> GetOrderStatus(Order o)
        {
            return await HttpGet<Order>(string.Format("venues/{0}/stocks/{1}/orders/{2}", o.Venue, o.Symbol, o.Id));
        }

        public async Task<OrderStatuses> GetOrderStatuses(string venue, string account)
        {
            return await HttpGet<OrderStatuses>(string.Format("venues/{0}/accounts/{1}/orders", venue, account));
        }

        public async Task<OrderStatuses> GetOrderStatuses(string venue, string account, string symbol)
        {
            return await HttpGet<OrderStatuses>(string.Format("venues/{0}/accounts/{1}/stocks/{2}/orders", venue, account, symbol));
        }

        public async Task<Order> CancelOrder(Order o)
        {
            using (var api = GetHttpClient())
            {
                try
                {
                    var orderUrl = string.Format("venues/{0}/stocks/{1}/orders/{2}", o.Venue, o.Symbol, o.Id);
                    var response = await api.DeleteAsync(orderUrl).ConfigureAwait(false);
                    return await ProcessHttpResponse<Order>(response).ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    var order = new Order { Ok = false, Error = string.Format("Error: {0}", ex.Message) };
                    return order;
                }
            }
        }
    }
}
