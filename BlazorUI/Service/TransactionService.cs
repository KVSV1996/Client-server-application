using BlazorUI.Data;
using System.Net.Http.Headers;

namespace BlazorUI.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;

        public TransactionService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("Api");
        }

        public async Task UpdateTransactionAsync(Transaction transaction, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "/transactions")
            {
                Content = JsonContent.Create(transaction)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> CreateTransactionAsync(Transaction transaction, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/transactions")
            {
                Content = JsonContent.Create(transaction)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task DeleteTransactionAsync(int id, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/transactions/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
