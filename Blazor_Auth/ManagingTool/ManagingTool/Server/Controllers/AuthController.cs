using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ManagingTool.Shared.DTO;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;
using ManagingTool.Server;

[ApiController]
[Route("api/[controller]")]
public class Auth
{
    [Authorize(Policy = "AccountIdPolicy")]
    [HttpGet]
    public ErrorCode CheckToken()
    {
        return ErrorCode.None;
    }

    [HttpPost("Login")]
    public ManagingLoginResponse Login(ManagingLoginRequest request)
    {
        var response = new ManagingLoginResponse() { Result = ErrorCode.LoginFailed };

        if (!(request.Email.Equals("genie@com2us.com") && request.Password.Equals("1234")))
        {
            return response;
        }

        // 로그인 성공 시 토큰 생성
        var (accessToken, refreshToken) = TokenManager.CreateTokens(1);

        // DB가 연동되어있다면, 해당 유저의 RefreshToken 컬럼을
        // 새로 발급한 RefreshToken으로 Update해주는 과정이 필요

        // 응답 데이터 세팅
        response.Result = ErrorCode.None;
        response.Name = "지니";
        response.AccountId = 1;
        response.accessToken = accessToken;
        response.refreshToken = refreshToken;
        
        return response;
    }
}