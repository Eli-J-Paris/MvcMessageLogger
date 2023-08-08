namespace MvcMessageLogger.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get;  set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? BirthDay { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Message> Messages { get; } = new List<Message>();

    }
}
