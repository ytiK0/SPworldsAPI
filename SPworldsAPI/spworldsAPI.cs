using Newtonsoft.Json;
using SPworldsAPI.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SPworldsAPI
{


    public class SPWorldsApiClient
    {
        private readonly string _token;
        private readonly string _webhookUrl;
        private readonly HttpClient _httpClient;

        public SPWorldsApiClient(string id, string token, string webhookUrl)
        {
            _token = token;
            _webhookUrl = webhookUrl;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://spworlds.ru/api/public/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Base64Encode($"{id}:{_token}"));
        }

        private async Task<string> SendRequest(string route, Dictionary<string, object>? requestData = null)
        {
            HttpResponseMessage response;

            if (requestData == null)
                response = await _httpClient.GetAsync(route);
            else
            {
                var json = JsonConvert.SerializeObject(requestData);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(route, requestContent);
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<int> GetCardBalanceAsync()
        {
            var responseContent = await SendRequest("card");
            var balanceResponse = JsonConvert.DeserializeObject<CardBalanceResponse>(responseContent);
            return balanceResponse.balance;
        }

        public async Task<string> CreatePaymentRequestAsync(int amount, string redirectUrl, string data)
        {
            if (data.Length > 100)
                throw new Exception("Слишком много данных");
            var requestData = new Dictionary<string, object>
            {
                { "amount", amount },
                { "redirectUrl", redirectUrl },
                { "webhookUrl", _webhookUrl },
                { "data", data }
            };
            var responseContent = await SendRequest("payment", requestData);
            var paymentResponse = JsonConvert.DeserializeObject<PaymentRequestResponse>(responseContent);
            return paymentResponse.url;
        }

        public async void MakeTransferAsync(string receiver, int amount, string comment)
        {
            var requestData = new Dictionary<string, object>
            {
                { "receiver", receiver },
                { "amount", amount },
                { "comment", comment }
            };
            await SendRequest("transactions", requestData);
        }
        

        public async Task<string> GetDiscordNicknameAsync(string discordId)
        {
            var responseContent = await SendRequest($"users/{discordId}");
            var usernameResponse = JsonConvert.DeserializeObject<DiscordUsernameResponse>(responseContent);
            return usernameResponse.username;
        }

        private string Base64Encode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
    }
}