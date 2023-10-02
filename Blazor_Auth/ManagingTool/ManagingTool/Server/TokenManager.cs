
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManagingTool.Server;

public class TokenManager
{
    private static string _signingKey = "SigningKey_Com2us";

    // AccessToken과 RefreshToken 생성
    public static Tuple<string, string> CreateTokens(Int64 accountId)
    {
        var accessToken = CreateToken(true, accountId);
        var refreshToken = CreateToken(false, accountId);

        return new Tuple<string, string>(accessToken, refreshToken);
    }

    // 토큰 종류에 따라 유효시간을 정하여 생성
    public static string CreateToken(bool isAccessToken, Int64 accountId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // Signing Key와 만료기간 설정
        var key = Encoding.ASCII.GetBytes(_signingKey);
        var expires = isAccessToken ? DateTime.UtcNow.AddHours(1) : DateTime.UtcNow.AddHours(6);

        // 토큰 구조체 정의
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim("AccountId", accountId.ToString()),
            }),
            Expires = expires,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // 토큰 생성
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // 토큰에 담긴 정보(Claim) 추출
    public static Int64 GetClaim(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var refreshToken = handler.ReadJwtToken(token);

        // AccountId Claim 추출
        var accountIdClaim = refreshToken.Claims.FirstOrDefault(claim => claim.Type.Equals("AccountId"));

        if (accountIdClaim != null)
        {
            return Int64.Parse(accountIdClaim.Value);
        }

        return 0;
    }
}