using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Front.Connection
{
    public partial class ApiConnection
    {
        public async Task<MemoryInfoResponse> GetMemoryInfo()
        {
            var url = $"/account/memory";
            var response = await SendGetRequestWithoutData<MemoryInfoResponse>(url);
            return response!; //TODO handle null properly
        }
    }
}
