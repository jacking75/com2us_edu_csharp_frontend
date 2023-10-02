using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPIServer.ReqRes;
using WebAPIServer.DbOperations;
using WebAPIServer.Util;
using ZLogger;

namespace WebAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AbortStage : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;
    readonly IRedisDb _redisDb;

    public AbortStage(ILogger<Login> logger, IGameDb gameDb, IRedisDb redisDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<AbortStageResponse> Post(AbortStageRequest request)
    {
        var response = new AbortStageResponse();

        var errorCode = await _redisDb.DeleteStageProgressDataAsync(request.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "AbortStage Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
