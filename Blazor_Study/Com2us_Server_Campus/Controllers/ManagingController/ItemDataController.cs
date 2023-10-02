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
public class ItemData : ControllerBase
{
    readonly ILogger<ItemData> _logger;
	readonly IGameDb _gameDb;
	readonly IMasterDb _masterDb;


	public ItemData(ILogger<ItemData> logger, IGameDb gameDb, IMasterDb masterDb)
    {
		_logger = logger;
		_gameDb = gameDb;
		_masterDb = masterDb;
    }

    [HttpPost("GetItemTable")]
	public async Task<GetItemTableResponse> Post(GetItemTableRequest request)
	{
		var response = await _masterDb.GetItemTableAsync();

		if (response.Item1 != ErrorCode.None)
		{
			return null;
		}

		return response.Item2;
	}

    [HttpPost("GetUserItemList")]
    public async Task<GetUserItemListResponse> Post(GetUserItemListRequest request)
    {
        var response = await _gameDb.GetUserItemListAsync(request.SearchType, request.SearchValue);

        if (response.errorCode != ErrorCode.None)
        {
            return null;
        }

        return response;
    }

    [HttpPost("RetrieveUserItem")]
    public async Task<RetrieveUserItemResponse> Post(RetrieveUserItemRequest request)
    {
        var response = await _gameDb.RetrieveUserItemAsync(request.SelectedItemList, request.MailForm);

        if (response.errorCode != ErrorCode.None)
        {
            return null;
        }

        return response;
    }
}
