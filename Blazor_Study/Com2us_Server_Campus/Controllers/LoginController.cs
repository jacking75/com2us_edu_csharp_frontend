using WebAPIServer.DbOperations;
using WebAPIServer.ReqRes;
using WebAPIServer.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using ZLogger;
using WebAPIServer.DataClass;

namespace WebAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IAccountDb _accountDb;
    readonly IRedisDb _redisDb;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;

    public Login(ILogger<Login> logger, IAccountDb accountDb, IRedisDb redisDb, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _accountDb = accountDb;
        _redisDb = redisDb;
        _gameDb = gameDb;
        _masterDb = masterDb;
    }

    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequest request)
    {
        var response = new LoginResponse();
        response.Result = ErrorCode.None;

        // 로그인 정보 검증
        var (errorCode, accountId) = await _accountDb.VerifyAccountAsync(request.Email, request.Password);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { Email = request.Email }, "Login Error");

            response.Result = errorCode;
            return response;
        }

        // 버전 데이터 검증
        errorCode = await _masterDb.VerifyVersionDataAsync(request.AppVersion, request.MasterVersion);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { Email = request.Email }, "Login Error");

            response.Result = errorCode;
            return response;
        }

        // 인증키 생성
        (errorCode, response.Authtoken) = await _redisDb.CreateUserAuthAsync(request.Email, accountId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { Email = request.Email }, "Login Error");

            response.Result = errorCode;
            return response;
        }

        // 기본 데이터 로딩
        (errorCode, response.userData) = await _gameDb.UserDataLoading(accountId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { Email = request.Email }, "Login Error");

            response.Result = errorCode;
            return response;
        }

        // 아이템 로딩
        (errorCode, response.userItem) = await _gameDb.UserItemLoadingAsync(response.userData.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = response.userData.UserId }, "Login Error");

            response.Result = errorCode;
            return response;
        }

        // 공지 읽어오기
        (errorCode, response.notificationUrl) = await _redisDb.LoadNotification();
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = response.userData.UserId }, "Login Error");

            response.Result = errorCode;
            return response;
        }

        // 채팅로비 접속
        (errorCode, response.LobbyNum) = await _redisDb.EnterChatLobbyFromLoginAsync(response.userData.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = response.userData.UserId }, "Login Error");

            response.Result = errorCode;
        }

        return response;
    }
}