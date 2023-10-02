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
public class ReceiveItemFromMail : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;

    public ReceiveItemFromMail(ILogger<Login> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<ReceiveItemFromMailResponse> Post(ReceiveItemFromMailRequest request)
    {
        var response = new ReceiveItemFromMailResponse();

        (var errorCode, response.itemInfo) = await _gameDb.ReceiveMailItemAsync(request.MailId, request.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, MailId = request.MailId }, "ReceiveItemFromMail Error");
            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
