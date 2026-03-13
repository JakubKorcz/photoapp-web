namespace PhotoApp.Common.ModelsShared
{
    public class ServerAuthResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string Username { get; set; }
    }
}
