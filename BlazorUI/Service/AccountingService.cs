using BlazorUI.Data;
using System.Net.Http.Headers;

namespace BlazorUI.Service
{
    public class AccountingService : IAccountingService
    {

        private readonly HttpClient _httpClient;

        public AccountingService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("Api");
        }

        public async Task<FinancialReport?> GetTransactionsForPeriodAsync(DateOnly startDate, DateOnly endDate, string token)
        {
            string startDateString = startDate.ToString("yyyy-MM-dd");
            string endDateString = endDate.ToString("yyyy-MM-dd");

            var requestUrl = $"/accounting/interval?startDate={startDateString}&endDate={endDateString}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<FinancialReport>();
        }
    }
}
