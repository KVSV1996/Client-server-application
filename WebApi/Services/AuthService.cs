using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Models;
using WebApi.Repository.Interface;
using WebApi.Services.Interface;

namespace WebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private IAuthRepository _authRepository;

        public AuthService(IConfiguration configuration, IAuthRepository authRepository )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
        }

        public string GenerateToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public IEnumerable<UserInfo> GetAllUsers()
        {
            return _authRepository.GetAllUsers().Select(userEntity => new UserInfo
            {
                Username = userEntity.Username,
                Password = null,
                Role = userEntity.Role
            }).ToList();
        }
        public UserEntity GetUserByUsername(string username) => _authRepository.GetUserByUsername(username);

        public void DeleteUser(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            _authRepository.DeleteUser(username);
            _authRepository.Save();
        }

        public void UpdateEntity(UserInfo userInformation)
        {
            var entity = _authRepository.GetUserByUsername(userInformation.Username);

            if (entity != null)
            {
                if (!string.IsNullOrEmpty(userInformation.Password))
                {
                    var (salt, hash) = HashPassword(userInformation.Password);
                    entity.PasswordSalt = Convert.ToBase64String(salt);
                    entity.PasswordHash = Convert.ToBase64String(hash);
                }
                entity.Role = userInformation.Role;

                _authRepository.UpdateUser(entity);
                _authRepository.Save();
            }
        }

        public void MakeUserEntity(UserInfo userInformation)
        {
            var (salt, hash) = HashPassword(userInformation.Password);
            var saltBase64 = Convert.ToBase64String(salt);
            var hashBase64 = Convert.ToBase64String(hash);


            var userInfo = new UserEntity
            {
                Username = userInformation.Username,
                PasswordSalt = saltBase64,
                PasswordHash = hashBase64,
                Role = userInformation.Role

            };

            _authRepository.InsertUser(userInfo);
            _authRepository.Save();
        }

        public bool VerifyPassword(string enteredPassword, string storedSaltBase64, string storedHashBase64)
        {
            int iterations = 10000;
            int hashSize = 20;

            byte[] storedSalt = Convert.FromBase64String(storedSaltBase64);
            byte[] storedHash = Convert.FromBase64String(storedHashBase64);

            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, storedSalt, iterations))
            {
                byte[] testHash = pbkdf2.GetBytes(hashSize);
                return testHash.SequenceEqual(storedHash);
            }
        }

        private (byte[] Salt, byte[] Hash) HashPassword(string password)
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
