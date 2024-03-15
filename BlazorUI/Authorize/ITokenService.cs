using BlazorUI.Data;

namespace BlazorUI.Authorize
{
    public interface ITokenService
    {
        string? Token { get; }
        int Role { get; }
        void SetToken(LoginResult loginResult);
    }
}
