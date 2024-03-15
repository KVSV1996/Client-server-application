using BlazorUI.Data;

namespace BlazorUI.Authorize
{
    public interface IAuthService
    {
        Task<LoginResult?> LoginAsync(string username, string password);
        Task<bool> CreateUserAsync(UserInfo userInfo, string token);
        Task<List<UserInfo>> GetAllUsersAsync(string token);
        Task<bool> DeleteUserAsync(string username, string token);
        Task<bool> UpdateUserAsync(UserInfo userInfo, string token);
    }
}
