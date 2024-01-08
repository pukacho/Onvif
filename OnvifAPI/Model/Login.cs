namespace OnvifAPI.Model
{
    public class Login
    {
        public required string Password { get; set; }
        public required string Username { get; set; }
        public int Id { get;  set; }
    }
}
