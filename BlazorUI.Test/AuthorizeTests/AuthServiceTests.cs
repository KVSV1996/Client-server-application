using BlazorUI.Data;
using BlazorUI.Service;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace BlazorUI.Test.AuthorizeTests
{
    [TestClass]
    public class AuthServiceTests
    {

        private MockHttpMessageHandler _mockHttp;
        private HttpClient _httpClient;
        private AuthService _authService;

        public AuthServiceTests()
        {
            _mockHttp = new MockHttpMessageHandler();
            _httpClient = _mockHttp.ToHttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7177/");

            var clientFactory = Mock.Of<IHttpClientFactory>(mock =>
                mock.CreateClient(It.IsAny<string>()) == _httpClient);

            _authService = new AuthService(clientFactory);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginResult()
        {
            // Arrange
            var username = "testUser";
            var password = "testPass";
            var expectedLoginResult = new LoginResult { Token = "fakeToken", Role = 1 };
            _mockHttp.When(HttpMethod.Post, "https://localhost:7177/api/Auth/login")
                     .Respond("application/json", JsonSerializer.Serialize(expectedLoginResult));

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedLoginResult.Token, result.Token);
            Assert.AreEqual(expectedLoginResult.Role, result.Role);
        }

        [TestMethod]
        public async Task LoginAsync_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var username = "wrongUser";
            var password = "wrongPass";
            _mockHttp.When(HttpMethod.Post, "https://localhost:7177/api/Auth/login")
                     .Respond(HttpStatusCode.Unauthorized);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateUserAsync_ValidUserInfo_ReturnsTrue()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "newUser", Password = "newPassword", Role = UserRole.User };
            var token = "testToken";
            _mockHttp.When(HttpMethod.Post, "https://localhost:7177/api/Auth/register")
                     .Respond(HttpStatusCode.OK);

            // Act
            var result = await _authService.CreateUserAsync(userInfo, token);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CreateUserAsync_InvalidUserInfo_ThrowsHttpRequestException()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "invalidUser", Password = "invalidPass", Role = UserRole.User };
            var token = "validToken";
            _mockHttp.When(HttpMethod.Post, "https://localhost:7177/api/Auth/register")
                     .Respond(HttpStatusCode.BadRequest);

            // Act & Assert
            Exception? exception = null;
            bool result = false;
            try
            {
                result = await _authService.CreateUserAsync(userInfo, token);
            }
            catch (HttpRequestException ex)
            {
                exception = ex;
            }

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public async Task CreateUserAsync_Unauthorized_ThrowsHttpRequestException()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "user", Password = "pass", Role = UserRole.User };
            var token = "invalidToken";
            _mockHttp.When(HttpMethod.Post, "https://localhost:7177/api/Auth/register")
                     .Respond(HttpStatusCode.Unauthorized);

            // Act
            bool result = false;
            HttpRequestException caughtException = null;

            try
            {
                result = await _authService.CreateUserAsync(userInfo, token);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(caughtException);
            Assert.AreEqual(HttpStatusCode.Unauthorized, caughtException.StatusCode);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ValidToken_ReturnsListOfUsers()
        {
            // Arrange
            var token = "validToken";
            var expectedUsers = new List<UserInfo>
            {
                new UserInfo { Username = "user1", Password = "password1", Role = UserRole.User },
                new UserInfo { Username = "user2", Password = "password2", Role = UserRole.Admin }
            };

            _mockHttp.When(HttpMethod.Get, "https://localhost:7177/api/Auth")
                .Respond("application/json", JsonSerializer.Serialize(expectedUsers));

            // Act
            var result = await _authService.GetAllUsersAsync(token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUsers.Count, result.Count);
            for (int i = 0; i < expectedUsers.Count; i++)
            {
                Assert.AreEqual(expectedUsers[i].Username, result[i].Username);
                Assert.AreEqual(expectedUsers[i].Role, result[i].Role);
            }
        }

        [TestMethod]
        public async Task GetAllUsersAsync_Unauthorized_ThrowsHttpRequestException()
        {
            // Arrange
            var token = "invalidToken";
            _mockHttp.When(HttpMethod.Get, "https://localhost:7177/api/Auth")
                     .Respond(HttpStatusCode.Unauthorized);

            // Act
            List<UserInfo> result = null;
            HttpRequestException caughtException = null;

            try
            {
                result = await _authService.GetAllUsersAsync(token);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(caughtException);
            Assert.AreEqual(HttpStatusCode.Unauthorized, caughtException.StatusCode);
        }

        [TestMethod]
        public async Task DeleteUserAsync_ValidUser_ReturnsTrue()
        {
            // Arrange
            var username = "existingUser";
            var token = "validToken";
            _mockHttp.When(HttpMethod.Delete, $"https://localhost:7177/api/Auth/{username}")
                     .Respond(HttpStatusCode.OK);

            // Act
            var result = await _authService.DeleteUserAsync(username, token);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteUserAsync_InvalidUser_ReturnsFalse()
        {
            // Arrange
            var username = "nonExistingUser";
            var token = "validToken";
            _mockHttp.When(HttpMethod.Delete, $"https://localhost:7177/api/Auth/{username}")
                     .Respond(HttpStatusCode.NotFound);

            // Act
            bool result = false;
            Exception? exception = null;
            try
            {
                result = await _authService.DeleteUserAsync(username, token);
            }
            catch (HttpRequestException ex)
            {
                exception = ex;
            }

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public async Task DeleteUserAsync_Unauthorized_ThrowsHttpRequestException()
        {
            // Arrange
            var username = "existingUser";
            var token = "invalidToken";
            _mockHttp.When(HttpMethod.Delete, $"https://localhost:7177/api/Auth/{username}")
                     .Respond(HttpStatusCode.Unauthorized);

            // Act
            HttpRequestException caughtException = null;

            try
            {
                await _authService.DeleteUserAsync(username, token);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsNotNull(caughtException);
            Assert.AreEqual(HttpStatusCode.Unauthorized, caughtException.StatusCode);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ValidUserInfo_ReturnsTrue()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "validUser", Password = "newPassword", Role = UserRole.User };
            var token = "validToken";
            _mockHttp.When(HttpMethod.Put, "https://localhost:7177/api/Auth")
                     .Respond(HttpStatusCode.OK);

            // Act
            var result = await _authService.UpdateUserAsync(userInfo, token);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdateUserAsync_InvalidUserInfo_ReturnsFalse()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "invalidUser", Password = "wrongPassword", Role = UserRole.User };
            var token = "validToken";
            _mockHttp.When(HttpMethod.Put, "https://localhost:7177/api/Auth")
                     .Respond(HttpStatusCode.BadRequest);

            // Act
            Exception? exception = null;
            try
            {
                await _authService.UpdateUserAsync(userInfo, token);
            }
            catch (HttpRequestException ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public async Task UpdateUserAsync_Unauthorized_ReturnsFalse()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "existingUser", Password = "correctPassword", Role = UserRole.Admin };
            var token = "invalidToken";
            _mockHttp.When(HttpMethod.Put, "https://localhost:7177/api/Auth")
                     .Respond(HttpStatusCode.Unauthorized);

            // Act
            HttpRequestException caughtException = null;

            try
            {
                await _authService.UpdateUserAsync(userInfo, token);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsNotNull(caughtException);
            Assert.AreEqual(HttpStatusCode.Unauthorized, caughtException.StatusCode);
        }
    }
}
