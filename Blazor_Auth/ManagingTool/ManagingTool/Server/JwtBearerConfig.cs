using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManagingTool.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ManagingTool.Server
{
    public class JwtBearerConfig
    {
        public TokenValidationParameters tokenValidatedParameters { get; }

        public JwtBearerConfig()
        {
            // 인증 옵션 파라미터 정의
            tokenValidatedParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,    // 토큰 유효성 검증 여부
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SigningKey_Com2us")) // 비밀 서명 키
            };
        }

        public void OnAuthenticationFailedHandler(AuthenticationFailedContext context, JwtBearerOptions options)
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                // 리프레시 토큰 가져오기
                if (GetRefreshToken(context, out var refreshToken) == false)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                // 리프레시 토큰이 만료되었는지 확인
                if (IsExpiredToken(context, refreshToken, options))
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                // 리프레시 토큰에서 AccountId 가져오기
                var accountId = TokenManager.GetClaim(refreshToken);
                if (accountId == 0)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                // DB의 RefreshToken과 비교
                if (AreEqualWithDBRefreshToken(accountId, refreshToken) == false)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                // 새 액세스 토큰 발급
                string newAccessToken = TokenManager.CreateToken(true, accountId);
                context.Response.Headers.Add("X-NEW-ACCESS-TOKEN", newAccessToken);

                // 요청 정상 수행을 위해 Authorization에서 사용할 ClaimsPrincipal을 Context에 추가
                ClaimsIdentity claims = new ClaimsIdentity(new[]
                {
                    new Claim("AccountId", accountId.ToString()),
                }, JwtBearerDefaults.AuthenticationScheme);

                context.Principal = new ClaimsPrincipal(new ClaimsIdentity[] { claims });

                context.Success();
            }
        }

        bool GetRefreshToken(AuthenticationFailedContext context, out string token)
        {
            token = string.Empty;

            // 헤더에서 리프레시 토큰을 가져옴
            var headerToken = context.Request.Headers["refresh_token"].FirstOrDefault();
            if (headerToken == null)
            {
                return false;
            }

            token = headerToken;
            return true;
        }

        bool IsExpiredToken(AuthenticationFailedContext context, string token, JwtBearerOptions options)
        {
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, options.TokenValidationParameters,
                                                            out var validatedToken);

                // 리프레시 토큰의 만료 시간 확인
                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return true;
            }
        }

        bool AreEqualWithDBRefreshToken(Int64 accountId, string headerRefreshToken)
        {
            // 실제로는 DB 검증 로직이 들어감
            return true;
        }
    }
}
