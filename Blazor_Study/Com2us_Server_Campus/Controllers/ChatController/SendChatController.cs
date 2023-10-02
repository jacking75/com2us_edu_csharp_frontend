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
public class SendChat : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IRedisDb _redisDb;

    public SendChat(ILogger<Login> logger, IRedisDb redisDb)
    {
        _logger = logger;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<SendChatResponse> Post(SendChatRequest request)
    {
        var response = new SendChatResponse();

        var errorCode = await _redisDb.SendChatAsync(request.UserId, request.Message);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, LobbyNum = request.LobbyNum }, "SendChat Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}