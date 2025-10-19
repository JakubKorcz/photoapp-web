using System.Net.Http;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection : IDisposable
    {
        private readonly HttpClient _httpClient;
        public ApiConnection(HttpClient httpClient) {
            _httpClient = httpClient;
        }
        public void Dispose() { }
    }
}
