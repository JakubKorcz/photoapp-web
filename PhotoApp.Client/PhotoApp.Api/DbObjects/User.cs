namespace PhotoApp.Api.DbObjects
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public int LoginCode { get; set; }
        public DateTime CodeExpiration { get; set; }
    }
}
