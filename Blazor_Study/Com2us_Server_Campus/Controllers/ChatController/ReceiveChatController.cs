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
public class ReceiveChat : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IRedisDb _redisDb;

    public ReceiveChat(ILogger<Login> logger, IRedisDb redisDb)
    {
        _logger = logger;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<ReceiveChatResponse> Post(ReceiveChatRequest request)
    {
        var response = new ReceiveChatResponse();

        (var errorCode, response.ChatHistory) = await _redisDb.ReceiveChatAsync(request.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "ReceiveChat Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}