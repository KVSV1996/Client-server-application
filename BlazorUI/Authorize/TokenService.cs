using BlazorUI.Data;

namespace BlazorUI.Authorize
{
    public class TokenService : ITokenService
    {
        public string? Token { get; private set; }
        public int Role { get; private set; }
        public void SetToken(LoginResult loginResult)
        {
            Token = loginResult.Token;
            Role = loginResult.Role;
        }
    }
}
