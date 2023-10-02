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
public class OpenMailBox : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;

    public OpenMailBox(ILogger<Login> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<OpenMailBoxResponse> Post(OpenMailBoxRequest request)
    {
        var response = new OpenMailBoxResponse();

        (var errorCode, response.mailData) = await _gameDb.LoadMailDataAsync(request.UserId, request.PageNumber);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, PageNumber = request.PageNumber }, "OpenMailBox Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
