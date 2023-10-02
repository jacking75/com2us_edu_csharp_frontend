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
public class ClearStage : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;
    readonly IRedisDb _redisDb;

    public ClearStage(ILogger<Login> logger, IGameDb gameDb, IRedisDb redisDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<ClearStageResponse> Post(ClearStageRequest request)
    {
        var response = new ClearStageResponse();

        (var errorCode, var itemList, var stageCode) = await _redisDb.CheckStageClearDataAsync(request.UserId);
        if (errorCode != ErrorCode.None)
        {
            if (errorCode != ErrorCode.CheckStageClearDataFailNotComplete)
            {
                _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "ClearStage Error");

                await _redisDb.DeleteStageProgressDataAsync(request.UserId);
            }
            
            response.Result = errorCode;
            return response;
        }

        (errorCode, response.itemInfo, response.ObtainedExp) = await _gameDb.ReceiveStageClearRewardAsync(request.UserId, stageCode, itemList);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "ClearStage Error");

            await _redisDb.DeleteStageProgressDataAsync(request.UserId);

            response.Result = errorCode;
            return response;
        }

        errorCode = await _gameDb.UpdateStageClearDataAsync(request.UserId, stageCode, request.ClearRank, request.ClearTime);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "ClearStage Error");

            await _redisDb.DeleteStageProgressDataAsync(request.UserId);

            response.Result = errorCode;
            return response;
        }

        await _redisDb.DeleteStageProgressDataAsync(request.UserId);

        return response;
    }
}
