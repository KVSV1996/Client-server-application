using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi.Models;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController( IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserInfo loginInfo)
        {
            var user = _authService.GetUserByUsername(loginInfo.Username);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var result = _authService.VerifyPassword(loginInfo.Password, user.PasswordSalt, user.PasswordHash);

            if (result)
            {
                var token = _authService.GenerateToken(user.Username);

                return Ok(new { token, role = (int)user.Role });
            }
            else
            {
                return Unauthorized("Invalid credentials.");
            }
        }

        [HttpPost("register")]
        [Authorize]
        public IActionResult Registeration(UserInfo loginInfo)
        {
            Log.Information("Attempting to create a new user");

            if (loginInfo == null)
            {
                Log.Warning("Attempted to create a null user.");
                return BadRequest("User cannot be null.");
            }
            try
            {
                _authService.MakeUserEntity(loginInfo);
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    Log.Error(ex, "Duplication of data occurred while processing your request.");
                    return StatusCode(409, "Duplication of data occurred while processing your request. Please correct the model and repeat.");
                }
                Log.Warning(ex, "An error occurred while registering a new user.");
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while registering a new user.");
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                Log.Information("Getting all users");
                var users = _authService.GetAllUsers();
                if (users.Any())
                {
                    return Ok(users);
                }
                else
                {
                    Log.Warning("No users found.");
                    return NotFound("No users found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving users.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{username}")]
        [Authorize]
        public IActionResult Delete(string username)
        {
            Log.Information($"Attempting to delete user with username: {username}");
            try
            {
                _authService.DeleteUser(username);
                Log.Information($"User with username: {username} was deleted successfully.");
                return Ok($"User with username: {username} was deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while deleting user with username: {username}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult Put(UserInfo userInfo)
        {
            Log.Information($"Attempting to update user with username: {userInfo.Username}");
            try
            {
                _authService.UpdateEntity(userInfo);
                Log.Information($"User with username: {userInfo.Username} updated successfully.");
                return Ok($"User with username: {userInfo.Username} updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while updating user with username: {userInfo.Username}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
