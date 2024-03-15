namespace BlazorUI.Data
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
    public enum UserRole
    {
        User,
        Admin
    }
}
