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
public class KillEnemy : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IRedisDb _redisDb;

    public KillEnemy(ILogger<Login> logger, IRedisDb redisDb)
    {
        _logger = logger;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<KillEnemyResponse> Post(KillEnemyRequest request)
    {
        var response = new KillEnemyResponse();

        var errorCode = await _redisDb.KillEnemyAsync(request.UserId, request.EnemyCode);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, EnemyCode = request.EnemyCode }, "KillEnemy Error");

            await _redisDb.DeleteStageProgressDataAsync(request.UserId);

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
