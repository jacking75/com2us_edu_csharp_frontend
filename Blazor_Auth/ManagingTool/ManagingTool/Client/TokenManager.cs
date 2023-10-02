using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.JSInterop;

namespace ManagingTool.Client
{
    public class TokenManager
    {
        readonly IJSRuntime _jsRuntime;

        public TokenManager(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // 세션 스토리지에서 토큰들을 가져옴
        public async Task<(string, string)> GetTokensFromSessionStorage()
        {
            var accessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "accesstoken");
            var refreshToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "refreshtoken");
            return (accessToken, refreshToken);
        }

        // Response 헤더에 재발급 토큰이 있는지 확인하고, 세션 스토리지의 액세스 토큰을 갱신
        public async Task UpdateAccessTokenIfPresent(HttpResponseMessage res)
        {
            if (res.Headers.TryGetValues("X-NEW-ACCESS-TOKEN", out var newAccessTokenEnum))
            {
                var newAccessToken = newAccessTokenEnum.FirstOrDefault();
                if (newAccessToken != null || newAccessToken != string.Empty)
                {
                    await SetNewAccessTokenToSessionStorage(newAccessToken!);
                }
            }
        }

        // 세션 스토리지의 액세스 토큰을 갱신
        public async Task SetNewAccessTokenToSessionStorage(string token)
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "accesstoken", token);
        }
    }
}
