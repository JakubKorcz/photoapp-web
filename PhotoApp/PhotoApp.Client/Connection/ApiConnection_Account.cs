using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<MemoryInfoResponse> GetMemoryInfo()
        {
            var url = $"https://localhost:7003/account/memory";
            var response = await SendGetRequestWithoutData<MemoryInfoResponse>(url);
            return response!; //TODO handle null properly
        }
    }
}
