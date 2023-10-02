namespace WebAPIServer.Controllers.ManagingController;

using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ManagingTool.Shared.DTO;
using System.Net.Http;


[ApiController]
[Route("api/[controller]")]
public class ItemData : ControllerBase
{
    readonly ILogger<ItemData> _logger;
    readonly HttpClient _httpClient;

    public ItemData(ILogger<ItemData> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpPost("GetItemTable")]
	public async Task<GetItemTableResponse> Post(GetItemTableRequest request)
	{
        var response = await _httpClient.PostAsJsonAsync("Managing/ItemData/GetItemTable", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<GetItemTableResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO;
    }

    [HttpPost("GetUserItemList")]
    public async Task<GetUserItemListResponse> Post(GetUserItemListRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("Managing/ItemData/GetUserItemList", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<GetUserItemListResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO;
    }

    [HttpPost("RetrieveUserItem")]
    public async Task<RetrieveUserItemResponse> Post(RetrieveUserItemRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("Managing/ItemData/RetrieveUserItem", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<RetrieveUserItemResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO;
    }

}
