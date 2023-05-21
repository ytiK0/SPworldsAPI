using Newtonsoft.Json;
using SPworldsAPI.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SPworldsAPI
{


    public class SPWorldsApiClient
    {
        private readonly string _id;
        private readonly string _token;
        private readonly string _webhookUrl;
        private readonly HttpClient _httpClient;

        public SPWorldsApiClient(string id, string token, string webhookUrl)
        {
            _id = id;
            _token = token;
            _webhookUrl = webhookUrl;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://spworlds.ru/api/public/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Base64Encode($"{_id}:{_token}"));
        }

        public async Task<int> GetCardBalanceAsync()
        {
            var response = await _httpClient.GetAsync("card");
            var responceContent = await response.Content.ReadAsStringAsync();
            var balanceResponce = JsonConvert.DeserializeObject<CardBalanceResponce>(responceContent);
            return balanceResponce.balance;
        }

        public async Task<string> CreatePaymentRequestAsync(int amount, string redirectUrl, string data)
        {
            var request = new Dictionary<string, object>
            {
                { "amount", amount },
                { "redirectUrl", redirectUrl },
                { "webhookUrl", _webhookUrl },
                { "data", data }
            };
            var json = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("payment", requestContent);
            var responceContent = await response.Content.ReadAsStringAsync();
            var paymentResponce = JsonConvert.DeserializeObject<PaymentRequestResponce>(responceContent);
            return paymentResponce.url;
        }

        public async void MakeTransferAsync(string receiver, int amount, string comment)
        {
            var request = new Dictionary<string, object>
            {
                { "receiver", receiver },
                { "amount", amount },
                { "comment", comment }
            };
            var json = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transactions", requestContent);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Операция провалилась");
        }

        public async Task<string> GetDiscordNicknameAsync(string discordId)
        {
            var response = await _httpClient.GetAsync($"users/{discordId}");
            var responceContent = await response.Content.ReadAsStringAsync();
            var usernameResponce = JsonConvert.DeserializeObject<DiscordUsernameResponce>(responceContent);
            return usernameResponce.username;
        }

        private string Base64Encode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
    }
}