namespace Users.Types
{
    public class AuthRequest(string UserName, string Password)
    {
        public string UserName { get; set; } = UserName;
        public string Password { get; set; } = Password;
    }
}