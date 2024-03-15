using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Repository;

namespace WebApi.Tests.RepositoryTests
{
    [TestClass]
    public class AuthRepositoryTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<FinanceContext> _contextOptions;

        public AuthRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<FinanceContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new FinanceContext(_contextOptions);
            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"CREATE VIEW AllUsers AS SELECT Id FROM UserEntity;";

                viewCommand.ExecuteNonQuery();
            }

            context.AddRange(
                new UserEntity { Id = 1, Username = "admin", PasswordSalt = "9LI4dEMrmpkGqK4+W4uENg==", PasswordHash = "5iquvmvaRg54aEL4/DuV9tV7/Yc=", Role = UserRole.Admin },
                new UserEntity { Id = 2, Username = "user", PasswordSalt = "HNfViOWFJCqTJmte8E7Ijw==", PasswordHash = "Hid2jXsUCDAPoO1UYqgmHgGHgBw=", Role = UserRole.User }
            );
            context.SaveChanges();
        }

        FinanceContext CreateContext() => new FinanceContext(_contextOptions);

        public void Dispose() => _connection.Dispose();

        [TestMethod]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new AuthRepository(context);

            // Act
            var users = repository.GetAllUsers().ToList();

            // Assert
            Assert.AreEqual(2, users.Count);
        }

        [TestMethod]
        public void GetUserByUsername_ExistingUser_ShouldReturnUser()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new AuthRepository(context);

            var expectedUsername = "admin";

            // Act
            var user = repository.GetUserByUsername(expectedUsername);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(expectedUsername, user.Username);
        }

        [TestMethod]
        public void InsertUser_NewUser_ShouldAddUserSuccessfully()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new AuthRepository(context);
            var newUser = new UserEntity { Username = "newuser", PasswordSalt = "newsalt", PasswordHash = "newhash", Role = UserRole.User };

            // Act
            repository.InsertUser(newUser);
            context.SaveChanges();

            // Assert
            var insertedUser = repository.GetUserByUsername("newuser");
            Assert.IsNotNull(insertedUser);
            Assert.AreEqual("newuser", insertedUser.Username);
        }

        [TestMethod]
        public void UpdateUser_ExistingUser_ShouldUpdateUserSuccessfully()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new AuthRepository(context);

            var user = new UserEntity { Username = "newuser", PasswordSalt = "oldSalt", PasswordHash = "oldHash", Role = UserRole.User };
            repository.InsertUser(user);
            context.SaveChanges();

            user.PasswordSalt = "newSalt";
            user.PasswordHash = "newHash";

            // Act
            repository.UpdateUser(user);
            context.SaveChanges();
            var result = repository.GetUserByUsername("newuser");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("newSalt", result.PasswordSalt);
            Assert.AreEqual("newHash", result.PasswordHash);
        }

        [TestMethod]
        public void DeleteUser_ExistingUser_ShouldRemoveUserSuccessfully()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new AuthRepository(context);
            var existingUsername = "admin";

            // Act
            repository.DeleteUser(existingUsername);
            context.SaveChanges();
            var user = repository.GetUserByUsername(existingUsername);

            // Assert
            Assert.IsNull(user);
        }

    }
}
