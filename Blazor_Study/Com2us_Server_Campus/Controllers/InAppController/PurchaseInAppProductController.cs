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
public class PurchaseInAppProduct : ControllerBase
{
    readonly ILogger<Login> _logger;
    readonly IGameDb _gameDb;

    public PurchaseInAppProduct(ILogger<Login> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<PurchaseInAppProductResponse> Post(PurchaseInAppProductRequest request)
    {
        var response = new PurchaseInAppProductResponse();

        var errorCode = await _gameDb.PurchaseInAppProductAsync(request.UserId, request.PurchaseId, request.ProductCode);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { UserId = request.UserId, PurchaseId = request.PurchaseId }, "PurchaseInAppProduct Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}