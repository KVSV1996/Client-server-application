using WebApi.Models;

namespace WebApi.Services.Interface
{
    public interface IAuthService
    {
        string GenerateToken(string username);
        UserEntity GetUserByUsername(string username);
        IEnumerable<UserInfo> GetAllUsers();
        void DeleteUser(string username);
        void MakeUserEntity(UserInfo userInformation);
        bool VerifyPassword(string enteredPassword, string storedSaltBase64, string storedHashBase64);
        void UpdateEntity(UserInfo userInformation);

    }
}
