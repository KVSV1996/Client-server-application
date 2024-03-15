using Moq;
using WebApi.Models.Enum;
using WebApi.Models;
using WebApi.Repository.Interface;
using WebApi.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace WebApi.Tests.ServiceTests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IAuthRepository> _mockAuthRepository;
        private AuthService _authService;

        
        public AuthServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockAuthRepository = new Mock<IAuthRepository>();
            _authService = new AuthService(_mockConfiguration.Object, _mockAuthRepository.Object);
            
            _mockConfiguration.SetupGet(m => m[It.Is<string>(s => s == "Jwt:Key")]).Returns("7n2JS79YJgHfuD8tC3mK3Tgin2p99S6e");
        }

        [TestMethod]
        public void GenerateToken_ShouldReturnValidToken()
        {
            // Arrange
            string username = "testUser";

            // Act
            var token = _authService.GenerateToken(username);

            // Assert
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<UserEntity>
            {
                new UserEntity { Username = "admin", PasswordSalt = "9LI4dEMrmpkGqK4+W4uENg==", PasswordHash = "5iquvmvaRg54aEL4/DuV9tV7/Yc=", Role = UserRole.Admin },
                new UserEntity { Username = "user", PasswordSalt = "HNfViOWFJCqTJmte8E7Ijw==", PasswordHash = "Hid2jXsUCDAPoO1UYqgmHgGHgBw=", Role = UserRole.User }
            };
            _mockAuthRepository.Setup(repo => repo.GetAllUsers()).Returns(users.AsEnumerable());

            // Act
            var result = _authService.GetAllUsers().ToList();

            // Assert
            Assert.AreEqual(users.Count, result.Count);
            for (int i = 0; i < users.Count; i++)
            {
                Assert.AreEqual(users[i].Username, result[i].Username);
                Assert.AreEqual(users[i].Role, result[i].Role);
            }
        }

        [TestMethod]
        public void GetUserByUsername_ExistingUser_ShouldReturnCorrectUser()
        {
            // Arrange
            var expectedUser = new UserEntity { Username = "existingUser", Role = UserRole.User };
            _mockAuthRepository.Setup(repo => repo.GetUserByUsername("existingUser")).Returns(expectedUser);

            // Act
            var result = _authService.GetUserByUsername("existingUser");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("existingUser", result.Username);
            Assert.AreEqual(UserRole.User, result.Role);
        }

        [TestMethod]
        public void DeleteUser_ExistingUser_ShouldCallDeleteUser()
        {
            // Arrange
            string username = "existingUser";
            _mockAuthRepository.Setup(repo => repo.DeleteUser(username));

            // Act
            _authService.DeleteUser(username);

            // Assert
            _mockAuthRepository.Verify(repo => repo.DeleteUser(username), Times.Once);
            _mockAuthRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [TestMethod]
        public void UpdateEntity_UserExists_ShouldUpdateUserCorrectly()
        {
            // Arrange
            var existingUser = new UserEntity
            {
                Username = "existingUser",
                Role = UserRole.User,
                PasswordSalt = "oldSalt",
                PasswordHash = "oldHash"
            };

            var updatedUserInfo = new UserInfo
            {
                Username = "existingUser",
                Password = "newPassword",
                Role = UserRole.Admin
            };

            _mockAuthRepository.Setup(repo => repo.GetUserByUsername("existingUser")).Returns(existingUser);
            _mockAuthRepository.Setup(repo => repo.UpdateUser(It.IsAny<UserEntity>()));
            _mockAuthRepository.Setup(repo => repo.Save());

            // Act
            _authService.UpdateEntity(updatedUserInfo);

            // Assert
            _mockAuthRepository.Verify(repo => repo.GetUserByUsername("existingUser"), Times.Once);
            _mockAuthRepository.Verify(repo => repo.UpdateUser(It.Is<UserEntity>(user =>
                user.Username == "existingUser" &&
                user.Role == UserRole.Admin &&
                !string.IsNullOrEmpty(user.PasswordSalt) &&
                !string.IsNullOrEmpty(user.PasswordHash)
            )), Times.Once);
            _mockAuthRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [TestMethod]
        public void MakeUserEntity_ValidUserInfo_ShouldCreateAndSaveUserCorrectly()
        {
            // Arrange
            var userInfo = new UserInfo
            {
                Username = "newUser",
                Password = "password123",
                Role = UserRole.User
            };

            _mockAuthRepository.Setup(repo => repo.GetUserByUsername(It.IsAny<string>())).Returns((UserEntity)null);
            _mockAuthRepository.Setup(repo => repo.InsertUser(It.IsAny<UserEntity>()));
            _mockAuthRepository.Setup(repo => repo.Save());

            // Act
            _authService.MakeUserEntity(userInfo);

            // Assert
            _mockAuthRepository.Verify(repo => repo.InsertUser(It.Is<UserEntity>(user =>
                user.Username == userInfo.Username &&
                !string.IsNullOrEmpty(user.PasswordSalt) &&
                !string.IsNullOrEmpty(user.PasswordHash) &&
                user.Role == userInfo.Role
            )), Times.Once);

            _mockAuthRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [TestMethod]
        public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            string correctPassword = "testPassword";
            string saltBase64;
            string hashBase64;

            using (var deriveBytes = new Rfc2898DeriveBytes(correctPassword, 16, 10000))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] hash = deriveBytes.GetBytes(20);

                saltBase64 = Convert.ToBase64String(salt);
                hashBase64 = Convert.ToBase64String(hash);
            }

            // Act
            bool isValid = _authService.VerifyPassword(correctPassword, saltBase64, hashBase64);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void VerifyPassword_WrongPassword_ShouldReturnFalse()
        {
            // Arrange
            string correctPassword = "testPassword";
            string wrongPassword = "wrongPassword";
            string saltBase64;
            string hashBase64;

            using (var deriveBytes = new Rfc2898DeriveBytes(correctPassword, 16, 10000))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] hash = deriveBytes.GetBytes(20);

                saltBase64 = Convert.ToBase64String(salt);
                hashBase64 = Convert.ToBase64String(hash);
            }

            // Act
            bool isValid = _authService.VerifyPassword(wrongPassword, saltBase64, hashBase64);

            // Assert
            Assert.IsFalse(isValid);
        }
    }
}
