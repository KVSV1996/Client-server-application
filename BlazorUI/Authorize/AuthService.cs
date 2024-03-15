using BlazorUI.Authorize;
using BlazorUI.Data;
using System.Net.Http.Headers;

namespace BlazorUI.Service
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("Api") ;
        }

        public async Task<LoginResult?> LoginAsync(string username, string password)
        {
            var loginData = new { username, password };
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(UserInfo userInfo, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/register")
            {
                Content = JsonContent.Create(userInfo)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserInfo>> GetAllUsersAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Auth");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var info = await response.Content.ReadFromJsonAsync<List<UserInfo>>() ?? throw new ArgumentNullException();

            return info;
        }

        public async Task<bool> DeleteUserAsync(string username, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Auth/{username}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(UserInfo userInfo, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/Auth")
            {
                Content = JsonContent.Create(userInfo)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

    }
}
