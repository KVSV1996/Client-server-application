using WebApi.Data;
using WebApi.Models;
using WebApi.Repository.Interface;

namespace WebApi.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private IFinanceContext context;

        public AuthRepository(IFinanceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.context = context;
        }
        public IEnumerable<UserEntity> GetAllUsers()
        {

            if (context.UserEntity == null)
            {
                throw new NullReferenceException();
            }

            return context.UserEntity;
        }

        public UserEntity? GetUserByUsername(string username)
        {
            if (context.UserEntity == null)
            {
                throw new ArgumentNullException(nameof(context.UserEntity));
            }

            return context.UserEntity.FirstOrDefault(user => user.Username == username);
        }

        public void InsertUser(UserEntity userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(userEntity));
            }

            context.UserEntity.Add(userEntity);
        }
        public void UpdateUser(UserEntity userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(userEntity));
            }

            context.UserEntity.Update(userEntity);
        }

        public void DeleteUser(string username)
        {
            UserEntity? user = context.UserEntity.FirstOrDefault(user => user.Username == username);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            context.UserEntity.Remove(user);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
