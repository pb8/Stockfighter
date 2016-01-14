using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using pb8.Stockfighter.Model;

namespace pb8.Stockfighter
{
    public class GameMaster
    {
        private readonly Uri _apiBaseUri;
        private readonly string _apiKey;

        public GameMaster(string apiKey)
        {
            _apiKey = apiKey;
            _apiBaseUri = new Uri("https://api.stockfighter.io/gm/");
        }

        public GameMaster(string apiKey, string apiBaseUrl)
        {
            _apiKey = apiKey;
            _apiBaseUri = new Uri(apiBaseUrl);
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

        private async Task<T> HttpPost<T>(string requestUri) where T : ResponseBase
        {
            using (var client = GetHttpClient())
            {
                try
                {
                    var response = await client.PostAsync(requestUri, new StringContent("")).ConfigureAwait(false);
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

        public async Task<LevelDetails> Start(string levelName)
        {
            return await HttpPost<LevelDetails>(string.Format("levels/{0}", levelName));
        }

        public async Task<LevelDetails> Restart(string instanceId)
        {
            return await HttpPost<LevelDetails>(string.Format("instances/{0}/restart", instanceId));
        }

        public async Task<ResponseBase> Stop(string instanceId)
        {
            return await HttpPost<ResponseBase>(string.Format("instances/{0}/stop", instanceId));
        }

        public async Task<LevelDetails> Resume(string instanceId)
        {
            return await HttpPost<LevelDetails>(string.Format("instances/{0}/resume", instanceId));
        }

        public async Task<LevelStatus> GetLevelStatus(string instanceId)
        {
            return await HttpGet<LevelStatus>(string.Format("instances/{0}", instanceId));
        }
    }
}
