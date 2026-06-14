using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Front.Client.Connection;

public partial class ApiConnection
{
    public async Task<ApiResult<MemoryInfoResponse>> GetMemoryInfo()
    {
        var url = $"account/memory";
        return await SendGetRequestWithoutData<MemoryInfoResponse>(url);
    }
}
