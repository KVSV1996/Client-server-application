using WebApi.Models;

namespace WebApi.Repository.Interface
{
    public interface IAuthRepository
    {
        IEnumerable<UserEntity> GetAllUsers();
        UserEntity? GetUserByUsername(string username);
        void InsertUser(UserEntity userEntity);
        void UpdateUser(UserEntity userEntity);
        void DeleteUser(string username);
        void Save();
    }
}
