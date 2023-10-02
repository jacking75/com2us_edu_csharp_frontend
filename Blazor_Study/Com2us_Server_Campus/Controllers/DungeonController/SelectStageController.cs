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
public class SelectStage : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;
    readonly IRedisDb _redisDb;

    public SelectStage(ILogger<Login> logger, IGameDb gameDb, IRedisDb redisDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<SelectStageResponse> Post(SelectStageRequest request)
    {
        var response = new SelectStageResponse();

        (var errorCode, response.stageItem, response.stageEnemy) = await _gameDb.VerifySelectedStageAsync(request.UserId, request.StageCode);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, StageCode = request.StageCode }, "SelectStage Error");
             
            response.Result = errorCode;
            return response;
        }

        errorCode = await _redisDb.CreateStageProgressDataAsync(request.UserId, request.StageCode);
        if (errorCode != ErrorCode.None)
        {
            if (errorCode != ErrorCode.CreateStageProgressDataFailInProgress)
            {
                _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, StageCode = request.StageCode }, "SelectStage Error");

                response.stageItem = null;
                response.stageEnemy = null;
            }

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
