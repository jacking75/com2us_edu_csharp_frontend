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
public class ObtainItem : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IRedisDb _redisDb;
    
    public ObtainItem(ILogger<Login> logger, IRedisDb redisDb)
    {
        _logger = logger;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<ObtainItemResponse> Post(ObtainItemRequest request)
    {
        var response = new ObtainItemResponse();

        var errorCode = await _redisDb.ObtainItemAsync(request.UserId, request.ItemCode, request.ItemCount);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, ItemCode = request.ItemCode, ItemCount = request.ItemCount }, "ObtainItem Error");

            await _redisDb.DeleteStageProgressDataAsync(request.UserId);

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
