using System.ComponentModel.DataAnnotations;
using WebAPIServer.DbOperations;
using WebAPIServer.ReqRes;
using WebAPIServer.Util;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;

namespace WebAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class SelectChatLobby : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IRedisDb _redisDb;

    public SelectChatLobby(ILogger<Login> logger, IRedisDb redisDb)
    {
        _logger = logger;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<SelectChatLobbyResponse> Post(SelectChatLobbyRequest request)
    {
        var response = new SelectChatLobbyResponse();

        (var errorCode, response.ChatHistory) = await _redisDb.SelectChatLobbyAsync(request.UserId, request.LobbyNum);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, LobbyNum = request.LobbyNum }, "SelectChatLobby Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}