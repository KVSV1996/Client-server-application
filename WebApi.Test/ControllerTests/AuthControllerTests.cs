using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApi.Controllers;
using WebApi.Models;
using WebApi.Models.Enum;
using WebApi.Services.Interface;

namespace WebApi.Tests.ControllerTests
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService;
        private AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        #region Login

        [TestMethod]
        public void Login_UserNotFound_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginInfo = new UserInfo { Username = "nonexistentUser", Password = "testPass" };
            _mockAuthService.Setup(service => service.GetUserByUsername("nonexistentUser")).Returns((UserEntity)null);

            // Act
            var result = _controller.Login(loginInfo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Login_InvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginInfo = new UserInfo { Username = "user", Password = "wrongPassword" };
            var userEntity = new UserEntity { Username = "user", PasswordSalt = "salt", PasswordHash = "hash" };
            _mockAuthService.Setup(service => service.GetUserByUsername("user")).Returns(userEntity);
            _mockAuthService.Setup(service => service.VerifyPassword("wrongPassword", "salt", "hash")).Returns(false);

            // Act
            var result = _controller.Login(loginInfo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Login_ValidCredentials_ShouldReturnOk()
        {
            // Arrange
            var loginInfo = new UserInfo { Username = "validUser", Password = "correctPassword" };
            var userEntity = new UserEntity { Username = "validUser", PasswordSalt = "salt", PasswordHash = "hash", Role = UserRole.User };
            _mockAuthService.Setup(service => service.GetUserByUsername("validUser")).Returns(userEntity);
            _mockAuthService.Setup(service => service.VerifyPassword("correctPassword", "salt", "hash")).Returns(true);
            _mockAuthService.Setup(service => service.GenerateToken("validUser")).Returns("token");

            // Act
            var actionResult = _controller.Login(loginInfo);

            // Assert
            Assert.IsNotNull(actionResult);

            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockAuthService.Verify(service => service.GetUserByUsername("validUser"), Times.Once);
            _mockAuthService.Verify(service => service.VerifyPassword("correctPassword", "salt", "hash"), Times.Once);
            _mockAuthService.Verify(service => service.GenerateToken("validUser"), Times.Once);
        }


        #endregion

        #region Registration

        [TestMethod]
        public void Registration_ValidUserInfo_ShouldReturnOk()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "newUser", Password = "password123", Role = UserRole.User };

            // Act
            var result = _controller.Registeration(userInfo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            _mockAuthService.Verify(service => service.MakeUserEntity(userInfo), Times.Once);
        }

        [TestMethod]
        public void Registration_NullUserInfo_ShouldReturnBadRequest()
        {
            // Arrange
            UserInfo userInfo = null;

            // Act
            var result = _controller.Registeration(userInfo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("User cannot be null.", badRequestResult.Value);
        }

        [TestMethod]
        public void Registration_ThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "newUser", Password = "password123", Role = UserRole.User };
            _mockAuthService.Setup(service => service.MakeUserEntity(It.IsAny<UserInfo>())).Throws(new Exception("Test exception"));

            // Act
            var result = _controller.Registeration(userInfo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request. Please try again later.", objectResult.Value);
        }

        #endregion

        #region Get

        [TestMethod]
        public void Get_WhenUsersExist_ShouldReturnOkWithUsers()
        {
            // Arrange
            var users = new List<UserInfo>
        {
            new UserInfo { Username = "user1", Role = UserRole.User },
            new UserInfo { Username = "user2", Role = UserRole.Admin }
        };
            _mockAuthService.Setup(service => service.GetAllUsers()).Returns(users);

            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var resultUsers = okResult.Value as IEnumerable<UserInfo>;
            Assert.AreEqual(2, resultUsers.Count());
        }

        [TestMethod]
        public void Get_WhenNoUsersExist_ShouldReturnNotFound()
        {
            // Arrange
            _mockAuthService.Setup(service => service.GetAllUsers()).Returns(new List<UserInfo>());

            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("No users found.", notFoundResult.Value);
        }

        [TestMethod]
        public void Get_WhenExceptionOccurs_ShouldReturnInternalServerError()
        {
            // Arrange
            _mockAuthService.Setup(service => service.GetAllUsers()).Throws(new Exception("Exception"));

            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", objectResult.Value);
        }

        #endregion

        #region Delete

        [TestMethod]
        public void Delete_ExistingUser_ShouldReturnOk()
        {
            // Arrange
            var username = "existingUser";
            _mockAuthService.Setup(service => service.DeleteUser(username));

            // Act
            var result = _controller.Delete(username);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"User with username: {username} was deleted successfully.", okResult.Value);
            _mockAuthService.Verify(service => service.DeleteUser(username), Times.Once);
        }

        [TestMethod]
        public void Delete_WhenExceptionOccurs_ShouldReturnInternalServerError()
        {
            // Arrange
            var username = "problemUser";
            _mockAuthService.Setup(service => service.DeleteUser(username)).Throws(new Exception("Exception"));

            // Act
            var result = _controller.Delete(username);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", objectResult.Value);
        }

        #endregion

        #region Put

        [TestMethod]
        public void Put_ValidUser_ShouldReturnOk()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "existingUser", Password = "newPassword", Role = UserRole.User };
            _mockAuthService.Setup(service => service.UpdateEntity(It.IsAny<UserInfo>()));

            // Act
            var result = _controller.Put(userInfo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"User with username: {userInfo.Username} updated successfully.", okResult.Value);
            _mockAuthService.Verify(service => service.UpdateEntity(It.Is<UserInfo>(u => u.Username == userInfo.Username)), Times.Once);
        }

        [TestMethod]
        public void Put_WhenExceptionOccurs_ShouldReturnInternalServerError()
        {
            // Arrange
            var userInfo = new UserInfo { Username = "problemUser", Password = "password", Role = UserRole.Admin };
            _mockAuthService.Setup(service => service.UpdateEntity(It.IsAny<UserInfo>())).Throws(new Exception("Test exception"));

            // Act
            var result = _controller.Put(userInfo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request.", objectResult.Value);
        }

        #endregion
    }
}
