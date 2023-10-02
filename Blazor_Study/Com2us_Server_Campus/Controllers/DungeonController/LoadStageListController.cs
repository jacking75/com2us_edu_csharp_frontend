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
public class LoadStageList : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;

    public LoadStageList(ILogger<Login> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<LoadStageListResponse> Post(LoadStageListRequest request)
    {
        var response = new LoadStageListResponse();

        (var errorCode, response.ClearStage) = await _gameDb.LoadStageListAsync(request.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "LoadStageList Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
