using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using ManagingTool.Shared.DTO;

namespace ManagingTool.Client;

public class AuthService : BaseService
{
    public static HttpClient _httpClient { get; set; }

    public AuthService(TokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    public async Task<ErrorCode> CheckToken()
    {
        // request 생성
        var requestMessage = await CreateReqMsg(HttpMethod.Get, "api/Auth", null);

        // response 받아오기
        var response = await _httpClient.SendAsync(requestMessage);
        await _tokenManager.UpdateAccessTokenIfPresent(response);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode != HttpStatusCode.OK)
        {
            return ErrorCode.Unauthorized;
        }

        return ErrorCode.None;
    }

    public async Task<ManagingLoginResponse> Login(string email, string pwd)
    {
        // request 생성
        var requestBody = new ManagingLoginRequest
        {
            Email = email,
            Password = pwd
        };

        var requestMessage = await CreateReqMsg(HttpMethod.Post, "api/Auth/Login", requestBody, false);

        // response 받아오기
        var response = await _httpClient.SendAsync(requestMessage);

        // 성공
        if (response.IsSuccessStatusCode)
        {
            var responseDTO = await response.Content.ReadFromJsonAsync<ManagingLoginResponse>();
            if (responseDTO == null)
            {
                return new ManagingLoginResponse { Result = ErrorCode.EmptyResponse };
            }
            return responseDTO;
        }

        // 실패
        else
        {
            return new ManagingLoginResponse { Result = ErrorCode.LoginRequestFail };
        }
    }
}
