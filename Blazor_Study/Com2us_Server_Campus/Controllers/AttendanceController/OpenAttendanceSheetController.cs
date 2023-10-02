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
public class OpenAttendanceSheet : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;

    public OpenAttendanceSheet(ILogger<Login> logger, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
    }

    [HttpPost]
    public async Task<OpenAttendanceSheetResponse> Post(OpenAttendanceSheetRequest request)
    {
        var response = new OpenAttendanceSheetResponse();
        
        (var errorCode, response.attendanceCount, response.IsNewAttendance) = await _gameDb.LoadAttendanceDataAsync(request.UserId);

        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId }, "OpenAttendanceSheet Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}