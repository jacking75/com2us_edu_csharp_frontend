using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using WebAPIServer.DataClass;
using WebAPIServer.DbOperations;
using WebAPIServer.Util;
using ZLogger;


namespace WebAPIServer.Middleware;

public class CheckUserAuth
{
    readonly RequestDelegate _next;
    readonly ILogger<CheckUserAuth> _logger;
    readonly IRedisDb _redisDb;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;

    public CheckUserAuth(RequestDelegate next, ILogger<CheckUserAuth> logger, IRedisDb redisDb, IGameDb gameDb, IMasterDb masterDb)
    {
        _next = next;
        _logger = logger;
        _redisDb = redisDb;
        _gameDb = gameDb;
        _masterDb = masterDb;
    }

    public async Task Invoke(HttpContext context)
    {
        var formString = context.Request.Path.Value;
        if (string.Compare(formString, "/CreateAccount", StringComparison.OrdinalIgnoreCase) == 0 ||
            string.Compare(formString, "/Login", StringComparison.OrdinalIgnoreCase) == 0)
        {
            await _next(context);

            return;
        }

        context.Request.EnableBuffering();

        Int64 accountId;
        string authToken;
        double appVersion;
        double masterVersion;
        string userLockKey = "";

        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
        {
            var bodyStr = await reader.ReadToEndAsync();

            // request 데이터 유무 확인
            if (IsNullBodyData(context, bodyStr))
            {
                await SetJsonResponse(context, ErrorCode.EmptyRequestHttpBody);
                return;
            }

            // request의 필요 데이터 존재 여부 확인
            var document = JsonDocument.Parse(bodyStr);
            if (IsInvalidJsonFormat(context, document, out accountId, out authToken, out appVersion, out masterVersion))
            {
                await SetJsonResponse(context, ErrorCode.InvalidRequestHttpBody);
                return;
            }

            // 게임 버전 데이터 검증
            if (await _masterDb.VerifyVersionDataAsync(appVersion, masterVersion) != ErrorCode.None)
            {
                await SetJsonResponse(context, ErrorCode.CheckUserGameDataNotMatch);
                return;
            }

            // User Regist 여부 확인
            var userInfo = await _redisDb.GetUserAuthAsync(accountId);
            if (userInfo == null)
            {
                await SetJsonResponse(context, ErrorCode.UserNotRegisted);
                return;
            }

            // AuthToken 확인
            if (IsInvalidUserAuthToken(context, userInfo, authToken))
            {
                await SetJsonResponse(context, ErrorCode.AuthTokenFailWrongAuthToken);
                return;
            }

            // Lock
            userLockKey = "ULock_" + accountId;
            if (await _redisDb.SetUserReqLockAsync(userLockKey))
            {
                await SetJsonResponse(context, ErrorCode.AuthTokenFailSetNx);
                return;
            }

            context.Items[nameof(UserAuth)] = userInfo;
        }

        context.Request.Body.Position = 0;

        await _next(context);
        await _redisDb.DelUserReqLockAsync(userLockKey);
    }

    public bool IsNullBodyData(HttpContext context, string bodyStr)
    {
        if (string.IsNullOrEmpty(bodyStr) == false)
        {
            return false;
        }

        return true;
    }

    public bool IsInvalidJsonFormat(HttpContext context, JsonDocument document, out Int64 accountId, out string authToken, out double appVersion, out double masterVersion)
    {
        try
        {
            accountId = document.RootElement.GetProperty("AccountId").GetInt64();
            authToken = document.RootElement.GetProperty("AuthToken").GetString();
            appVersion = document.RootElement.GetProperty("AppVersion").GetDouble();
            masterVersion = document.RootElement.GetProperty("MasterVersion").GetDouble();

            return false;
        }
        catch
        {
            accountId = 0; authToken = ""; appVersion = 0; masterVersion = 0;

            return true;
        }
    }

    public bool IsInvalidUserAuthToken(HttpContext context, UserAuth userInfo, string authToken)
    {
        if (string.CompareOrdinal(userInfo.AuthToken, authToken) == 0)
        {
            return false;
        }
        return true;
    }

    public async Task SetJsonResponse(HttpContext context, ErrorCode errorCode)
    {
        try
        {
            var JsonResponse = JsonSerializer.Serialize(new CheckUserAuthResponse
            {
                result = errorCode
            });

            var bytes = Encoding.UTF8.GetBytes(JsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), "CheckUserAuth Error");
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, "SetJsonResponse Exception");            
        }
    }
}

public class CheckUserAuthResponse
{
    public ErrorCode result { get; set; }
}