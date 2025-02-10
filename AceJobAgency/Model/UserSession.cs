namespace AceJobAgency.Model
{
    public class UserSession
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActiveAt { get; set; }
    }

}
