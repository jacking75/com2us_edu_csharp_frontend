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
public class ReadMail : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;

    public ReadMail(ILogger<Login> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<ReadMailResponse> Post(ReadMailRequest request)
    {
        var response = new ReadMailResponse();

        (var errorCode, response.Content, response.Item) = await _gameDb.ReadMailAsync(request.MailId, request.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, MailId = request.MailId }, "ReadMail Error");
            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
