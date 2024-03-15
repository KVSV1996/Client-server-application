using WebApi.Models.Enum;

namespace WebApi.Models
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
