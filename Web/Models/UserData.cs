namespace Web.Models
{
    public class UserData
    {
        public string User {  get; set; } = string.Empty;
        public List<string> Orders { get; set; } = [];
        public List<string> Products { get; set; } = [];
    }
}
