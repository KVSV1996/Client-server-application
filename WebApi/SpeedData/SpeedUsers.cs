using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebApi.Data;
using WebApi.Models.Enum;
using WebApi.Models;

namespace WebApi.SpeedData
{
    public class SeedUsers
    {
        public static void InitializeUser(IServiceProvider serviceProvider)
        {
            using (var context = new FinanceContext(serviceProvider.GetRequiredService<DbContextOptions<FinanceContext>>()))
            {

                if (!context.UserEntity.Any())
                {
                    var usersData = new List<UserInfo>
                    {
                        new UserInfo { Username = "admin", Password = "admin", Role = UserRole.Admin },
                        new UserInfo { Username = "user", Password = "user", Role = UserRole.User },
                    };

                    foreach (var userData in usersData)
                    {
                        var (generatedSalt, generatedHash) = HashPassword(userData.Password);

                        string saltBase64 = Convert.ToBase64String(generatedSalt);
                        string hashBase64 = Convert.ToBase64String(generatedHash);


                        var newUser = new UserEntity
                        {
                            Username = userData.Username,
                            PasswordSalt = saltBase64,
                            PasswordHash = hashBase64,
                            Role = userData.Role
                        };
                        context.UserEntity.Add(newUser);
                    }
                }
                context.SaveChanges();
            }
        }

        private static (byte[] Salt, byte[] Hash) HashPassword(string password)
        {
            int iterations = 10000;
            int hashSize = 20;
            byte[] salt = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] hash = pbkdf2.GetBytes(hashSize);
                return (salt, hash);
            }
        }
    }
}
