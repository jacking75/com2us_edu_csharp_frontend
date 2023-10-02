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
public class DeleteMail : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;

    public DeleteMail(ILogger<Login> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<DeleteMailResponse> Post(DeleteMailRequest request)
    {
        var response = new DeleteMailResponse();

        var errorCode = await _gameDb.DeleteMailAsync(request.MailId, request.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, MailId = request.MailId }, "DeleteMail Error");
            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
