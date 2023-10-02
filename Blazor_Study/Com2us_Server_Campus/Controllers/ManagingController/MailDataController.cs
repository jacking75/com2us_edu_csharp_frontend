namespace WebAPIServer.Controllers.ManagingController;

using WebAPIServer.DbOperations;
using WebAPIServer.ReqRes;
using WebAPIServer.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using ZLogger;
using WebAPIServer.DataClass;
using System.Net.Http;
using Org.BouncyCastle.Asn1.Ocsp;


[ApiController]
[Route("Managing/[controller]")]
public class MailData : ControllerBase
{
    readonly ILogger<MailData> _logger;
	readonly IGameDb _gameDb;
	readonly IMasterDb _masterDb;


	public MailData(ILogger<MailData> logger, IGameDb gameDb, IMasterDb masterDb)
    {
		_logger = logger;
		_gameDb = gameDb;
		_masterDb = masterDb;
    }

    [HttpPost("SendMail")]
	public async Task<SendMailResponse> Post(SendMailRequest request)
	{
		var response = await _gameDb.SendManagingMailAsync(request.MailForm, request.UserID);

		if (response.errorCode != ErrorCode.None)
		{
			return null;
		}

		return response;
	}

    [HttpPost("GetUserMailList")]
    public async Task<GetUserMailListResponse> Post(GetUserMailListRequest request)
    {
        var response = await _gameDb.GetUserMailListAsync(request.UserID);

        if (response.errorCode != ErrorCode.None)
        {
            return null;
        }

        return response;
    }
}
