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

[ApiController]
[Route("Managing/[controller]")]
public class UserData : ControllerBase
{
    readonly ILogger<UserData> _logger;
	readonly IGameDb _gameDb;


	public UserData(ILogger<UserData> logger, IGameDb gameDb)
    {
		_logger = logger;
		_gameDb = gameDb;
    }

    [HttpPost("GetUserBasicInfo")]
	public async Task<GetUserBasicInfoListResponse> Post(GetUserBasicInfoRequest request)
	{
		var response = await _gameDb.GetUserBasicInfoAsync(request.UserID);

		return response;
	}

	[HttpPost("GetMultipleUserBasicInfo")]
	public async Task<GetUserBasicInfoListResponse> Post(GetMultipleUserBasicInfoRequest request)
	{
		var response = await _gameDb.GetMultipleUserBasicInfoAsync(request.Category, request.MinValue, request.MaxValue);

		return response;
	}

    [HttpPost("UpdateUserBasicInfo")]
    public async Task<UpdateUserBasicInformationResponse> Post(UpdateUserBasicInformationRequest request)
    {
        var response = await _gameDb.UpdateUserBasicInfoAsync(request.UserInfo);

        return response;
    }
}
