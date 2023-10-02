using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace ManagingTool.Client
{
    public class BaseService
    {
        protected TokenManager _tokenManager;

        // request Message를 생성
        protected async Task<HttpRequestMessage> CreateReqMsg(HttpMethod method, string path, object? body, bool addHeader = true)
        {
            // Request Message 생성
            var requestMessage = new HttpRequestMessage(method, path);

            // Body 직렬화
            if (body != null)
            {
                SerializeReqBody(ref requestMessage, body);
            }

            // 헤더에 토큰 추가
            if (addHeader)
            {
                var (accessToken, refreshToken) = await _tokenManager.GetTokensFromSessionStorage();
                AttachTokensToRequestHeader(ref requestMessage, accessToken, refreshToken);
            }

            return requestMessage;
        }

        // Request Body를 JSON 직렬화하여 Body에 저장
        void SerializeReqBody(ref HttpRequestMessage reqMsg, object reqBody)
        {
            string requestBody = JsonSerializer.Serialize(reqBody);
            reqMsg.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        }

        // AccessToken과 RefreshToken을 RequestMessage 헤더에 추가
        protected void AttachTokensToRequestHeader(ref HttpRequestMessage req, string accessToken, string refreshToken)
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            req.Headers.Remove("refresh_token");
            req.Headers.Add("refresh_token", refreshToken);
        }
    }
}
