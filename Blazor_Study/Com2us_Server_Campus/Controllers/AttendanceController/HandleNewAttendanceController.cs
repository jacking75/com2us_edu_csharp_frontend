using System.ComponentModel.DataAnnotations;
using WebAPIServer.DbOperations;
using WebAPIServer.ReqRes;
using WebAPIServer.Util;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;

namespace WebAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class HandleNewAttendance : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;

    public HandleNewAttendance(ILogger<Login> logger, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
    }

    [HttpPost]
    public async Task<HandleNewAttendanceResponse> Post(HandleNewAttendanceRequest request)
    {
        var response = new HandleNewAttendanceResponse();
        
        var errorCode = await _gameDb.HandleNewAttendanceAsync(request.UserId);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "HandleNewAttendance Error");

            response.Result = errorCode;
            return response;
        }        

        return response;
    }
}